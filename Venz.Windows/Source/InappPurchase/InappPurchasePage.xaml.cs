using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Venz.Windows
{
    public partial class InappPurchasePage: Page
    {
        private PageContext Context = new PageContext();

        public InappPurchasePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            Context.Initialize(Settings.Create((String)args.Parameter));
            DataContext = Context;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);
            Context.Dispose();
        }

        private async void OnItemClicked(Object sender, ItemClickEventArgs args) => await Context.RequestPurchaseAsync((InappPurchaseItem)args.ClickedItem);

        public class Settings
        {
            public IList<InappPurchaseItem> Items { get; } = new List<InappPurchaseItem>();
            public Boolean GroupingEnabled { get; set; }
            public String Header { get; set; }
            public String Description { get; set; }

            public static Settings Create(String jsonValue)
            {
                var parameter = JsonObject.Parse(jsonValue);
                var instance = new Settings();
                instance.Header = parameter.GetNamedString("header");
                instance.Description = parameter.GetNamedString("description");
                instance.GroupingEnabled = Boolean.Parse(parameter.GetNamedString("grouping"));
                var index = 0;
                while (parameter.ContainsKey($"{index}"))
                    instance.Items.Add(new InappPurchaseItem(parameter.GetNamedString($"{index}"), parameter.GetNamedString($"{index}title"), parameter.GetNamedString($"{index}price"), parameter.GetNamedString($"{index++}group")));
                return instance;
            }

            public String GetNavigationParameter()
            {
                var jsonValue = new JsonObject();
                jsonValue.Add("header", JsonValue.CreateStringValue(Header));
                jsonValue.Add("description", JsonValue.CreateStringValue(Description));
                jsonValue.Add("grouping", JsonValue.CreateStringValue(GroupingEnabled.ToString()));
                for (var i = 0; i < Items.Count; i++)
                {
                    jsonValue.Add($"{i}", JsonValue.CreateStringValue(Items[i].Id));
                    jsonValue.Add($"{i}title", JsonValue.CreateStringValue(Items[i].Title));
                    jsonValue.Add($"{i}price", JsonValue.CreateStringValue(Items[i].Price));
                    jsonValue.Add($"{i}group", JsonValue.CreateStringValue(Items[i].Group));
                }
                return jsonValue.Stringify();
            }
        }

        private class PageContext: IDisposable
        {
            public InappPurchase InappPurchase { get; } = new InappPurchase();
            public String Header { get; private set; }
            public String Description { get; private set; }

            public void Initialize(Settings settings)
            {
                Header = settings.Header;
                Description = settings.Description;
                foreach (var item in settings.Items)
                    InappPurchase.AvailableItems.Add(item);
                InappPurchase.Syncronize();
            }

            public Task RequestPurchaseAsync(InappPurchaseItem item) => InappPurchase.RequestAsync(item);

            public void Dispose()
            {
                InappPurchase.Clean();
            }
        }

        public class PurchaseGroup: ObservableCollection<InappPurchaseItem>
        {
            public String Header { get; }
            public PurchaseGroup(String header, IEnumerable<InappPurchaseItem> items): base(items) { Header = header; }
        }
    }
}