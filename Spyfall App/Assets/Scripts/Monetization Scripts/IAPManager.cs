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
        } else {
            // show this on UI, add when making popup design
            Debug.Log("Purchasing currently unavailable. Check your internet connection and try again.");
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        this.controller = controller;
        manageGame.PaidUnlocked = GetProVersionPurchased();
    }

    // called when a purchase completes
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {
        // unlock functionality in PaidUnlocked setter
        manageGame.PaidUnlocked = GetProVersionPurchased();
        manageGame.purchasePopup.ClosePopup();
        return PurchaseProcessingResult.Complete;
    }

    public bool GetProVersionPurchased() {
        Product proVersion = controller.products.WithID(proVersionID);
        return proVersion != null && proVersion.hasReceipt;
    }

    public void OnInitializeFailed(InitializationFailureReason error) { }
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p) { }
}
