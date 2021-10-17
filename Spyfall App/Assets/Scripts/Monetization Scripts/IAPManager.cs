using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : IStoreListener {
    private IStoreController controller;
    private readonly string proVersionID = "pro_version";
    private readonly ManageGame manageGame;

    public IAPManager(ManageGame manageGame) {
        this.manageGame = manageGame;
        StandardPurchasingModule.Instance().useFakeStoreAlways = true;
        //StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.DeveloperUser;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(proVersionID, ProductType.NonConsumable);
        UnityPurchasing.Initialize(this, builder);
    }

    public void PurchaseProVersion() {
        if (controller != null) {
            controller.InitiatePurchase(proVersionID);
        } else if (manageGame.purchasePopup.gameObject.activeSelf) {
            manageGame.purchasePopup.SetButtonText(PurchasePopupController.ButtonMessage.unavailable);
        } else if (manageGame.purchasePopup2.gameObject.activeSelf) {
            manageGame.purchasePopup2.SetButtonText(PurchasePopupController.ButtonMessage.unavailable);
        }
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p) {
        if (manageGame.purchasePopup.gameObject.activeSelf) {
            manageGame.purchasePopup.SetButtonText(PurchasePopupController.ButtonMessage.whoops);
        } else if (manageGame.purchasePopup2.gameObject.activeSelf) {
            manageGame.purchasePopup2.SetButtonText(PurchasePopupController.ButtonMessage.whoops);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        this.controller = controller;
        manageGame.PaidUnlocked = GetProVersionPurchased();

        #if UNITY_IOS
            // restore transactions
            if (!manageGame.PaidUnlocked) {
                extensions.GetExtension<IAppleExtensions>().RestoreTransactions(result => {
                    if (result) {
                        Debug.Log("Restore Transactions Success");
                    } else {
                        Debug.Log("Restore Transactions Failure");
                    }
                });
            }
        #endif
    }

    // called when a purchase completes
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {
        // unlock functionality in PaidUnlocked setter
        manageGame.PaidUnlocked = GetProVersionPurchased();

        if (manageGame.purchasePopup.gameObject.activeSelf) {
            manageGame.purchasePopup.ClosePopup();
        }
        if (manageGame.purchasePopup2.gameObject.activeSelf) {
            manageGame.purchasePopup2.ClosePopup();
        }
        return PurchaseProcessingResult.Complete;
    }

    public bool GetProVersionPurchased() {
        Product proVersion = controller.products.WithID(proVersionID);
        return proVersion != null && proVersion.hasReceipt;
    }

    public void OnInitializeFailed(InitializationFailureReason error) { }
}
