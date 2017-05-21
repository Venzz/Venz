using System;

namespace Venz.Windows
{
    public class InappPurchaseItem
    {
        public String Id { get; }
        public String Title { get; }
        public String Price { get; }
        public String Group { get; }

        public InappPurchaseItem(String id, String title, String price)
        {
            Id = id;
            Title = title;
            Price = price;
            Group = "";
        }

        public InappPurchaseItem(String id, String title, String price, String group)
        {
            Id = id;
            Title = title;
            Price = price;
            Group = group;
        }
    }
}