using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;
using Point = System.Windows.Point;

namespace VPet.Plugin.ScratchCard;

public partial class Card : Window
{
    private ScratchCard main;
    private string cardName;

    private double percentToScratch = 0.85;
    private int scratchedPixels = -1;
    private int pixelsToScratch = 0;

    private Drop drop;
    private int brushRadius = 10;
    private bool isScratching = false;
    private Point? lastPosition = null;

    public Card(ScratchCard main, string cardName)
    {
        this.main = main;
        this.cardName = cardName;
        InitializeComponent();
        this.ChangeImage();
        this.SetUpWin();
        this.GetPixelInfo();
    }

    private void ChangeImage()
    {
        this.BackImage.Source = new BitmapImage(new Uri(Path.Combine(this.main.path, "cards", $"{this.cardName}_back.png")));
        this.FrontImage.Source = new BitmapImage(new Uri(Path.Combine(this.main.path, "cards", $"{this.cardName}_front.png")));
    }

    private void GetPixelInfo()
    {
        BitmapSource bitmapSource = this.FrontImage.Source as BitmapSource;
        if (bitmapSource == null) return;

        WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

        int stride = writeableBitmap.PixelWidth * (writeableBitmap.Format.BitsPerPixel / 8);
        byte[] pixels = new byte[stride * writeableBitmap.PixelHeight];
        writeableBitmap.CopyPixels(pixels, stride, 0);

        for (int i = 0; i < writeableBitmap.PixelHeight; i++)
            for (int j = 0; j < writeableBitmap.PixelWidth; j++)
            {
                int index = i * stride + j * 4;
                if (pixels[index + 3] != 0)
                    this.pixelsToScratch++;
            }
    }

    private void SetUpWin()
    {
        int rate = new Random().Next(1001);
        List<Drop> drops = (this.cardName == "scratchcard_normal") ? this.main.dropRates.normal : (this.cardName == "scratchcard_premium") ? this.main.dropRates.premium : this.main.dropRates.premiumPlus;
        Drop drop = drops.First(e => e.RateFrom <= rate && rate <= e.RateTo);
        if (drop == null) return;
        this.drop = drop;
        this.DropText.Text = drop.Text.Translate();
    }

    private void ScratchImage(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || !this.isScratching)
        {
            this.lastPosition = null;
            return;
        }

        if (this.scratchedPixels >= this.pixelsToScratch * this.percentToScratch)
        {
            this.scratchedPixels = 0;
            this.ScratchOff();
            return;
        }

        BitmapSource bitmapSource = this.FrontImage.Source as BitmapSource;
        if (bitmapSource == null) return;

        Point currentPosition = e.GetPosition(this.FrontImage);
        int x = (int)(currentPosition.X * (bitmapSource.PixelWidth / this.FrontImage.ActualWidth));
        int y = (int)(currentPosition.Y * (bitmapSource.PixelHeight / this.FrontImage.ActualHeight));

        WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);
        writeableBitmap.Lock();

        if (this.lastPosition.HasValue)
        {
            int lastX = (int)(this.lastPosition.Value.X * (bitmapSource.PixelWidth / this.FrontImage.ActualWidth));
            int lastY = (int)(this.lastPosition.Value.Y * (bitmapSource.PixelHeight / this.FrontImage.ActualHeight));
            this.BresenhamLine(lastX, lastY, x, y, writeableBitmap);
        }

        this.lastPosition = currentPosition;
        writeableBitmap.Unlock();
        this.FrontImage.Source = writeableBitmap;
    }

    private void BresenhamLine(int x0, int y0, int x1, int y1, WriteableBitmap writeableBitmap)
    {
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int err = dx - dy;

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        while (true)
        {
            this.ScratchAt(x0, y0, writeableBitmap);
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

    }

    private void ScratchAt(int x, int y, WriteableBitmap writeableBitmap)
    {
        for (int i = -this.brushRadius; i <= this.brushRadius; i++)
            for (int j = -this.brushRadius; j <= this.brushRadius; j++)
            {
                int targetX = x + i;
                int targetY = y + j;

                if (targetX < 0 || targetX >= writeableBitmap.PixelWidth) continue;
                if (targetY < 0 || targetY >= writeableBitmap.PixelHeight) continue;

                double distance = Math.Sqrt(i * i + j * j);
                if (distance > this.brushRadius) continue;

                unsafe
                {
                    IntPtr buffer = writeableBitmap.BackBuffer;
                    int stride = writeableBitmap.BackBufferStride;

                    byte* p = (byte*)(buffer + targetY * stride + targetX * 4);
                    if (p[3] != 0)
                        this.scratchedPixels++;
                    p[3] = 0;
                }
            }
    }

    private void ScratchOff()
    {
        if (this.drop.WinMoney != 0)
            this.main.MW.GameSavesData.GameSave.Money += this.drop.WinMoney;
        this.FrontImage.Visibility = Visibility.Collapsed;
        this.isScratching = false;

        if (drop.RateTo > 990)
            this.main.SendMsg(this.main.dialogue.big_win);
        else if (drop.RateTo > 800)
            this.main.SendMsg(this.main.dialogue.win);
        else if (drop.RateTo > 500)
            this.main.SendMsg(this.main.dialogue.small_win);
        else
            this.main.SendMsg(this.main.dialogue.losses);
    }

    private void ScratchImage_MouseEnter(object sender, MouseEventArgs e)
    {
        this.isScratching = true;
    }

    private void ScratchImage_MouseLeave(object sender, MouseEventArgs e)
    {
        this.isScratching = false;
        this.lastPosition = null;
    }

    private void Exit(object sender, MouseButtonEventArgs e)
    {
        if (this.FrontImage.Visibility == Visibility.Visible)
            if (MessageBox.Show("Are you sure you want to leave without scratching the scratch card?".Translate(), "Exit without scratching".Translate(), MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                return;
        this.Close();
    }

    private void DragCard(object sender, MouseButtonEventArgs e)
    {
        if (this.isScratching) return;
        this.DragMove();
    }
}
