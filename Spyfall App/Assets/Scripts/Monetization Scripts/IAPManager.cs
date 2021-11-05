using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPManager : IStoreListener {
    private IStoreController controller;
    private IExtensionProvider extensions;

    private readonly string proVersionID = "pro_version_2";

    private readonly ManageGame manageGame;

    public IAPManager(ManageGame manageGame) {
        this.manageGame = manageGame;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(proVersionID, ProductType.NonConsumable);

        // expect a response in OnInitialized or OnInitializeFailed
        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized() {
        return controller != null && extensions != null;
    }

    public void PurchaseProVersion() {
        BuyProductID(proVersionID);
    }

    private void BuyProductID(string productID) {
        if (IsInitialized()) {
            Product product = controller.products.WithID(productID);

            if (product != null && product.availableToPurchase) {

                Debug.Log($"Purchasing product asychronously: {product.definition.id}");
                // expect a reponse in ProcessPurchase or OnPurchaseFailed
                controller.InitiatePurchase(product);

            } else {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                UpdateButtonMessage(PurchasePopupController.ButtonMessage.whoops);
            }
        } else {
            Debug.Log("BuyProductID FAIL. Not initialized.");
            UpdateButtonMessage(PurchasePopupController.ButtonMessage.unavailable);
        }
    }

    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases() {
        if (!IsInitialized()) {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
            Debug.Log("RestorePurchases started ...");

            var apple = extensions.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        } else {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    private void UpdateButtonMessage(PurchasePopupController.ButtonMessage buttonMessage) {
        if (manageGame.purchasePopup.gameObject.activeSelf) {
            manageGame.purchasePopup.SetButtonText(buttonMessage);
        } else if (manageGame.purchasePopup2.gameObject.activeSelf) {
            manageGame.purchasePopup2.SetButtonText(buttonMessage);
        }
    }

    private bool IsReceiptValid(string unityIAPReceipt) {
        // IMPORTANT: GO BACK LATER AND ADD GOOGLE PLAY PUBLIC LICENSE KEY TO RECEIPT VALIDATION OBFUSCATOR
        // window -> unity iap -> receipt validation obfuscator
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        try {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            var result = validator.Validate(unityIAPReceipt);

            bool correctReceipt = false;
            // For informational purposes, we list the receipt(s)
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result) {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);

                correctReceipt |= productReceipt.productID == proVersionID;
            }

            if (!correctReceipt) {
                Debug.Log($"Validated receipts do not match {proVersionID}, locking all content");
                manageGame.LockAllContent();
                return false;
            }
        } catch (IAPSecurityException) {
            Debug.Log("Invalid receipt, locking all content");
            manageGame.LockAllContent();
            return false;
        }

        return true;
    }

    //
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        Debug.Log("On Initialized: PASS");

        this.controller = controller;
        this.extensions = extensions;

        // perform check that unlocked content has a valid receipt, otherwise lock all content
        IsReceiptValid(controller.products.WithID(proVersionID).receipt);
    }

    public void OnInitializeFailed(InitializationFailureReason error) {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    // called when a purchase completes
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
        bool validPurchase = IsReceiptValid(args.purchasedProduct.receipt);

        if (validPurchase) {
            Debug.Log($"ProcessPurchase: PASS. Product: {args.purchasedProduct.definition.id}");

            // unlock functionality in PaidUnlocked setter
            manageGame.PaidUnlocked = true;

            if (manageGame.purchasePopup.gameObject.activeSelf) {
                manageGame.purchasePopup.ClosePopup();
            }
            if (manageGame.purchasePopup2.gameObject.activeSelf) {
                manageGame.purchasePopup2.ClosePopup();
            }
        } else {
            Debug.Log("ProcessPurchase: FAIL. Invalid purchase caught by CrossPlatformValidator.");
            UpdateButtonMessage(PurchasePopupController.ButtonMessage.whoops);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
        Debug.Log($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
        UpdateButtonMessage(PurchasePopupController.ButtonMessage.whoops);
    }
}
