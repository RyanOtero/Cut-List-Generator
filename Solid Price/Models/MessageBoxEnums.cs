namespace Solid_Price.Models {
    public enum MessageBoxType {
        ConfirmationWithYesNo = 0,
        ConfirmationWithYesNoCancel,
        Information,
        Error,
        Warning
    }

    public enum MessageBoxImage {
        Warning = 0,
        Question,
        Information,
        Error,
        None
    }
}
