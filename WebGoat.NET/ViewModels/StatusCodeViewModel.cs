using System;
using System.ComponentModel.DataAnnotations;

namespace WebGoatCore.ViewModels
{
    public class StatusCodeViewModel
    {
        [Display(Name = "HTTP response code:")]
        public int Code { get; init; }

        [Display(Name = "Message:")]
        public string? Message { get; init; }

        public bool HasMessage => !string.IsNullOrEmpty(Message);

        public static StatusCodeViewModel Create(ApiResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            return new StatusCodeViewModel
            {
                Code = response.StatusCode,
                Message = response.Message
            };
        }
    }
}