using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Storage;

namespace Venz.Windows
{
    public class InappPurchase
    {
        private LicenseInformation LicenseInformation;
        private TaskScheduler Scheduler;

        public ObservableCollection<InappPurchaseItem> AvailableItems { get; }



        public InappPurchase()
        {
            Scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            #if DEBUG
            LicenseInformation = CurrentAppSimulator.LicenseInformation;
            #else
            LicenseInformation = CurrentApp.LicenseInformation;
            #endif
            LicenseInformation.LicenseChanged += LicenseInformation_LicenseChanged;
            AvailableItems = new ObservableCollection<InappPurchaseItem>();
        }

        public static async Task LoadFakeDataAsync(String fakeDataFilePath)
        {
            #if DEBUG
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(fakeDataFilePath, UriKind.Absolute)).AsTask().ConfigureAwait(false);
            await CurrentAppSimulator.ReloadSimulatorAsync(storageFile).AsTask().ConfigureAwait(false);
            #else
            await Task.FromResult<Object>(null);
            #endif
        }

        private void LicenseInformation_LicenseChanged()
        {
            var taskFactory = new TaskFactory(Scheduler);
            taskFactory.StartNew(() => Syncronize());
        }

        public void Syncronize()
        {
            foreach (var productLicense in LicenseInformation.ProductLicenses.Values)
            {
                if (productLicense.IsActive)
                {
                    var item = AvailableItems.FirstOrDefault(a => a.Id == productLicense.ProductId);
                    if (item != null)
                        AvailableItems.Remove(item);
                }
            }
        }

        public async Task RequestAsync(InappPurchaseItem item)
        {
            try
            {
                #if DEBUG
                await CurrentAppSimulator.RequestProductPurchaseAsync(item.Id).AsTask().ConfigureAwait(false);
                #else
                await CurrentApp.RequestProductPurchaseAsync(item.Id).AsTask().ConfigureAwait(false);
                #endif
            }
            catch (Exception)
            {
            }
        }

        public void Clean() => LicenseInformation.LicenseChanged -= LicenseInformation_LicenseChanged;

        public static Boolean IsAnyPurchased()
        {
            #if DEBUG
            var licenseInformation = CurrentAppSimulator.LicenseInformation;
            #else
            var licenseInformation = CurrentApp.LicenseInformation;
            #endif

            foreach (var productLicense in licenseInformation.ProductLicenses.Values)
                if (productLicense.IsActive)
                    return true;
            return false;
        }

        public static Boolean IsAnyPurchased(IEnumerable<String> products)
        {
            #if DEBUG
            var licenseInformation = CurrentAppSimulator.LicenseInformation;
            #else
            var licenseInformation = CurrentApp.LicenseInformation;
            #endif

            foreach (var productLicense in licenseInformation.ProductLicenses.Values)
                if (productLicense.IsActive && products.Contains(productLicense.ProductId))
                    return true;
            return false;
        }
    }
}
