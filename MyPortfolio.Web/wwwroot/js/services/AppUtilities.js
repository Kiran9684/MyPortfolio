export class AppUtilities {
    static spinner = $('#globalSpinner'); //spinner div container available in _layout.cshtml
    static enableGlobalSpinner() {
        this.spinner.toggleClass('d-none', false);
        this.spinner.toggleClass('d-flex', true);
    }

    static disableGlobalSpinner() {
        this.spinner.toggleClass('d-none', true);
        this.spinner.toggleClass('d-flex', false);
    }
}