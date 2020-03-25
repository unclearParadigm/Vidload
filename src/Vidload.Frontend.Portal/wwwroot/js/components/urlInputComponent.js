/**
 * Component managing state of the Media-URL input-field and the Dropdown for selection of the target-format.
 */
class UrlInputComponent {
  /**
   * @param {$} urlInputField
   * @param {$} formatDropdown
   * @param {$} containerElement
   * @param {$} inputErrorModal
   */
  constructor(urlInputField, formatDropdown, containerElement, inputErrorModal) {
    this.urlInputField = urlInputField;
    this.formatDropdown = formatDropdown;
    this.containerElement = containerElement;
    this.inputErrorModal = inputErrorModal;
  }

  initialize(onMediaDownloadRequestCallback) {
    this.stopError();
    this.stopLoading();
    this.setWriteable();
    this.clearMediaUrl();

    this.urlInputField.change((ev) => {
      const potentialMediaUrl = this.urlInputField.val().trim();
      const targetFormatValue = this.formatDropdown.dropdown('get value');
      if(ev.keyCode === 13 && this.isMediaUrlValid(potentialMediaUrl)) {
        this.setReadonly();
        this.indicateLoading();
        onMediaDownloadRequestCallback(potentialMediaUrl, targetFormatValue);
      } else {
        this.indicateError();
        this.inputErrorModal.modal('show');
      }
    });
  }

  setReadonly() {
    this.containerElement.addClass('disabled');
    this.formatDropdown.addClass('disabled');
  }

  setWriteable() {
    this.containerElement.removeClass('disabled');
    this.formatDropdown.removeClass('disabled');
  }

  indicateLoading() {
    this.containerElement.addClass('loading');
  }

  stopLoading() {
    this.containerElement.removeClass('loading');
  }

  indicateError() {
    this.containerElement.addClass('error');
  }

  stopError() {
    this.containerElement.removeClass('error');
  }

  clearMediaUrl() {
    this.urlInputField.val('');
  }

  isMediaUrlValid(urlString) {
    const pattern = new RegExp('^(https?:\\/\\/)?' + // protocol
      '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' + // domain name
      '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
      '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
      '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
      '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
    return !!pattern.test(urlString);
  }
}
