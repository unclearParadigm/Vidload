namespace VidloadPortal.Models {
  public class ResponseModel<T> {
    public bool IsSuccess { get; set; }
    public string Error { get; set; }

    public bool HasValue { get; set; }
    public T Data { get; set; }

    public static ResponseModel<T> CreateSuccess(T data) {
      return new ResponseModel<T> {Data = data, IsSuccess = true, HasValue = data != null};
    }

    public static ResponseModel<T> CreateFailure(string message) {
      return new ResponseModel<T> {Error = message, IsSuccess = false, HasValue = false};
    }
  }
}
