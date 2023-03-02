namespace APP
{
    public class SoapService : ISoapService
    {
        ASMXService.Service service;
        public List<Item> Items { get; private set; } = new List<Item>();

        public SoapService ()
        {
            service = new ASMXService.Service ();
            service.Url = Constants.SoapUrl;
            ...
        }

        ASMXService.Item ToASMXServiceItem (Item item)
        {
            return new ASMXService.Item {
                ID = item.ID,
                Name = item.Name,
                Value = item.Value,
                Done = item.Done
            };
        }

        static Item FromASMXServiceItem (ASMXService.Item item)
        {
            return new Item {
                ID = item.ID,
                Name = item.Name,
                 Value = item.Value,
                Done = item.Done
            };
        }

        TaskCompletionSource<bool> getRequestComplete = null;
 
        public SoapService()
        {
            service.GetItemsCompleted += Service_GetItemsCompleted;
        }

        public async Task<List<Item>> RefreshDataAsync()
        {
            getRequestComplete = new TaskCompletionSource<bool>();
            service.GetItemsAsync();
            await getRequestComplete.Task;
            return Items;
        }

        private void Service_GetItemsCompleted(object sender, ASMXService.GetItemsCompletedEventArgs e)
        {
            try
            {
                getRequestComplete = getRequestComplete ?? new TaskCompletionSource<bool>();

                Items = new List<Item>();
                foreach (var item in e.Result)
                {
                    Items.Add(FromASMXServiceItem(item));
                }
                getRequestComplete?.TrySetResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\t\tERROR {0}", ex.Message);
            }
        }
    }
}