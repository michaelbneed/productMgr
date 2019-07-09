using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PM.Auth.GraphApi;
using PM.Entity.Services;

namespace PM.Business.Email
{
    public class RequestEmail
    {
        private readonly IConfiguration _configuration;

        private readonly IDbReadService _dbReadService;

        public RequestEmail(IConfiguration configuration, IDbReadService dbReadService)
        {
            _configuration = configuration;
            _dbReadService = dbReadService;
        }

        public void SendNewRequestToHeadQuarters(Entity.Models.Request request)
        {
            var adminUrl = _configuration.GetValue<string>("AdminWebsite:BaseUrl");
            var requestEditPath = string.Format(_configuration.GetValue<string>("AdminWebsite:RequestEdit"), request.Id);
            var subject = $"Vandivier Product Request";
            var emailAddresses = new GraphClient(_configuration, false).GetGroupUsersEmail(_configuration.GetValue<string>("SecurityGroups:HeadQuarters"));

            foreach (var emailAddress in emailAddresses)
            {
                var body = $"A new product request has been added that requires your attention. <br /><br />" +
                           $"View the request <a href='{adminUrl}{requestEditPath}'>here</a> <br /><br />" +
                           $"Thanks, <br /> Vandivier Management";

                if (emailAddress != null)
                {
					Helper.Send(_configuration, subject, body, new List<string>() { emailAddress });
				}
                
            }
        }

        public async void SendRequestToSuppliers(Entity.Models.Request request)
        {
            var adminUrl = _configuration.GetValue<string>("AdminWebsite:BaseUrl");
            var requestEditPath = string.Format(_configuration.GetValue<string>("AdminWebsite:RequestEdit"), request.Id);
            var subject = $"Vandivier Product Request";
            var users = await _dbReadService.GetAllRecordsAsync<Entity.Models.User>(x => x.SupplierId == request.SupplierId.Value);

            foreach (var user in users)
            {
                var body = $"A new product request has been added that requires your attention. <br /><br />" +
                           $"View the request <a href='{adminUrl}{requestEditPath}'>here</a> <br /><br />" +
                           $"Thanks, <br /> Vandivier Management";

                if (user.EmailAddress != null)
                {
	                Helper.Send(_configuration, subject, body, new List<string>() { user.EmailAddress });
				}
            }
        }

        public void SendNewNoteEmailToHeadQuarters(Entity.Models.Request request, Entity.Models.Note note)
        {
            var adminUrl = _configuration.GetValue<string>("AdminWebsite:BaseUrl");
            var noteDetailPath = string.Format(_configuration.GetValue<string>("AdminWebsite:NoteDetail"), note.Id);
            var subject = $"Vandivier Product Request Note";
            var emailAddresses = new GraphClient(_configuration, false).GetGroupUsersEmail(_configuration.GetValue<string>("SecurityGroups:HeadQuarters"));

            foreach (var emailAddress in emailAddresses)
            {
                var body = $"A new note has been added for a product request that requires your attention. <br /><br />" +
                           $"View the note <a href='{adminUrl}{noteDetailPath}'>here</a> <br /><br />" +
                           $"Thanks, <br /> Vandivier Management";

                if (emailAddress != null)
                {
					Helper.Send(_configuration, subject, body, new List<string>() { emailAddress });
				}
            }
        }

        public void SendNewNoteEmailToStoreManagers(Entity.Models.Request request, Entity.Models.Note note)
        {
            var adminUrl = _configuration.GetValue<string>("AdminWebsite:BaseUrl");
            var noteDetailPath = string.Format(_configuration.GetValue<string>("AdminWebsite:NoteDetail"), note.Id);
            var subject = $"Vandivier Product Request Note";
            var emailAddresses = new GraphClient(_configuration, false).GetGroupUsersEmail(_configuration.GetValue<string>("SecurityGroups:StoreManager"));

            foreach (var emailAddress in emailAddresses)
            {
                var body = $"A new note has been added for a product request that requires your attention. <br /><br />" +
                           $"View the note <a href='{adminUrl}{noteDetailPath}'>here</a> <br /><br />" +
                           $"Thanks, <br /> Vandivier Management";

                if (emailAddress != null)
                {
					Helper.Send(_configuration, subject, body, new List<string>() { emailAddress });
				}
            }
        }

        public void SendNewNoteEmailToOriginatingUser(Entity.Models.Request request, Entity.Models.Note note)
        {
            var adminUrl = _configuration.GetValue<string>("AdminWebsite:BaseUrl");
            var noteDetailPath = string.Format(_configuration.GetValue<string>("AdminWebsite:NoteDetail"), note.Id);
            var subject = $"Vandivier Product Request Note";

            var emailAddress = new GraphClient(_configuration, false).GetUserEmail(request.UserId);
            var body = $"A new note has been added for a product request that requires your attention. <br /><br />" +
                        $"View the note <a href='{adminUrl}{noteDetailPath}'>here</a> <br /><br />" +
                        $"Thanks, <br /> Vandivier Management";

            if (emailAddress != null)
            {
	            Helper.Send(_configuration, subject, body, new List<string>() { emailAddress });
			}
        }

        public async void SendNewNoteEmailToSuppliers(Entity.Models.Request request, Entity.Models.Note note)
        {
            if (request.SupplierId.HasValue == false)
                return;

            var vendorUrl = _configuration.GetValue<string>("VendorWebsite:BaseUrl");
            var noteDetailPath = string.Format(_configuration.GetValue<string>("VendorWebsite:NoteDetail"), note.Id);
            var subject = $"Vandivier Product Request Note";
            var users = await _dbReadService.GetAllRecordsAsync<Entity.Models.User>(x => x.SupplierId == request.SupplierId.Value);

            foreach (var user in users)
            {
                var body = $"A new note has been added for a product request that requires your attention. <br /><br />" +
                           $"View the note <a href='{vendorUrl}{noteDetailPath}'>here</a> <br /><br />" +
                           $"Thanks, <br /> Vandivier Management";

                if (user.EmailAddress != null)
                {
	                Helper.Send(_configuration, subject, body, new List<string>() { user.EmailAddress });
				}
            }
        }
    }
}
