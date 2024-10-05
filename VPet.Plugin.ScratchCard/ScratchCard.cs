using LinePutScript.Localization.WPF;
using Panuon.WPF.UI;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.ScratchCard
{
    public class ScratchCard : MainPlugin
    {
        public string path;
        private int multiplier = 5;
        private int cardBuyed = 0;
        private bool isSay = true;
        private int attempts = 0;

        public Dialogue dialogue = new Dialogue();
        public DropRates dropRates = new DropRates();

        public override string PluginName => nameof(ScratchCard);

        public ScratchCard(IMainWindow mainwin)
          : base(mainwin)
        {
        }

        public override void LoadPlugin()
        {
            this.path = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
            this.CreateMenuItem();
            this.CreateBuyListing();
        }

        private async void CreateBuyListing()
        {
            WindowX winBetterBuy = null;
            foreach (WindowX winX in Application.Current.Windows)
                if (winX.ToString() == "VPet_Simulator.Windows.winBetterBuy")
                    winBetterBuy = winX;

            if (winBetterBuy == null)
            {
                await Task.Delay(1000);
                if (this.attempts <= 10)
                    CreateBuyListing();
                this.attempts++;
                return;
            }

            ItemsControl icCommodity = (ItemsControl)winBetterBuy.FindName("IcCommodity");
            if (icCommodity == null) return;

            icCommodity.ItemContainerGenerator.ItemsChanged += async (sender, args) =>
            {
                ItemContainerGenerator itemsControl = (sender as ItemContainerGenerator);
                await Task.Delay(100);
                foreach (var item in itemsControl.Items)
                {
                    ContentPresenter contentPresenter = itemsControl.ContainerFromItem(item) as ContentPresenter;
                    if (contentPresenter == null) continue;
                    Button button = FindChild<Button>(contentPresenter);
                    if (button == null) continue;
                    button.Click -= ItemBuyed;
                    button.Click += ItemBuyed;
                }
            };
        }

        private void ItemBuyed(object sender, RoutedEventArgs e)
        {
            Button Button = sender as Button;
            Food item = Button.DataContext as Food;
            if (item == null) return;
            string name = item.Name;
            if (!name.StartsWith("scratchcard_")) return;
            
            Card card = new Card(this, name);
            card.Show();
            this.cardBuyed++;

            if (this.cardBuyed % this.multiplier == 0)
            {
                this.cardBuyed = 0;
                this.multiplier = (int)(this.multiplier * 1.5);
                this.SendMsg(dialogue.abuse);
            }
        }

        public void SendMsg(string[] texts)
        {
            if (!this.isSay) return;
            int i = new Random().Next(texts.Length);
            string text = texts[i].Translate();
            this.MW.Main.Say(text);
        }

        private void CreateMenuItem()
        {
            MenuItem menuModConfig = this.MW.Main.ToolBar.MenuMODConfig;
            menuModConfig.Visibility = Visibility.Visible;

            MenuItem menuItem = new MenuItem()
            {
                Header = nameof(ScratchCard).Translate(),
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            MenuItem sayItem = new MenuItem()
            {
                Header = "Turn off messages".Translate(),
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            sayItem.Click += (RoutedEventHandler)((s, e) => this.ChangeIsSay(s, e));
            menuItem.Items.Add(sayItem);
            menuModConfig.Items.Add(menuItem);
        }

        private void ChangeIsSay(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            this.isSay = !this.isSay;
            menuItem.Header = this.isSay ? "Turn off messages" : "Turn on messages";
        }

        private T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}