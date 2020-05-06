using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PM.Auth.GraphApi;
using PM.Entity.Models;
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

        public void SendApprovedRequestEmailToHeadQuarters(Entity.Models.Request request)
        {
	        var adminUrl = _configuration.GetValue<string>("AdminWebsite:BaseUrl");
	        var requestEditPath = string.Format(_configuration.GetValue<string>("AdminWebsite:RequestEdit"), request.Id);
	        var subject = $"Vandivier Product Request";
	        var emailAddresses = new GraphClient(_configuration, false).GetGroupUsersEmail(_configuration.GetValue<string>("SecurityGroups:HeadQuarters"));

	        foreach (var emailAddress in emailAddresses)
	        {
		        var body = $"A product request has been approved and requires your attention. <br /><br />" +
		                   $"View the request <a href='{adminUrl}{requestEditPath}'>here</a> <br /><br />" +
		                   $"Thanks, <br /> Vandivier Management";

		        if (emailAddress != null)
		        {
			        Helper.Send(_configuration, subject, body, new List<string>() { emailAddress });
		        }

	        }
        }

        public void SendDeniedRequestEmailToOriginatingUser(Entity.Models.Request request)
        {
	        var subject = $"Vandivier Product Request: {request.RequestDescription}";
			var emailAddress = new GraphClient(_configuration, false).GetUserEmail(request.UserId);
	        var body = $"Your request {request.RequestDescription} was not approved at this time. <br /><br />" +
					   $"Thanks, <br /> Vandivier Management";

	        if (emailAddress != null)
	        {
		        Helper.Send(_configuration, subject, body, new List<string>() { emailAddress });
	        }
        }

        public void SendRequestCompletedToGroup(Entity.Models.Request request)
        {
	        SendRequestCompletedToOriginatingUser(request);
	        SendRequestCompletedToSupplier(request);
	        SendRequestCompletedToStoreManager(request);
	        SendRequestCompletedToHeadquarters(request);
		}

        private void SendRequestCompletedToSupplier(Request request)
        {
			if (request.SupplierId.HasValue == false)
				return;

			var vendorUrl = _configuration.GetValue<string>("VendorWebsite:BaseUrl");
			var requestEditPath = string.Format(_configuration.GetValue<string>("VendorWebsite:RequestEdit"), request.Id);
			var subject = $"Vandivier Product Request Complete: {request.RequestDescription}";
			var users = _dbReadService.GetAllRecordsAsync<Entity.Models.User>(x => x.SupplierId == request.SupplierId.Value).Result;

			foreach (var user in users)
			{
				var body = $"Request {request.RequestDescription} has been completed! <br /><br />" +
				           $"View the request <a href='{vendorUrl}{requestEditPath}'>here</a> <br /><br />" +
				           $"Thanks, <br /> Vandivier Management";

				if (user.EmailAddress != null)
				{
					Helper.Send(_configuration, subject, body, new List<string>() { user.EmailAddress });
				}
			}
        }

        private void SendRequestCompletedToHeadquarters(Request request)
        {
			var adminUrl = _configuration.GetValue<string>("AdminWebsite:BaseUrl");
			var requestEditPath = string.Format(_configuration.GetValue<string>("AdminWebsite:RequestEdit"), request.Id);
			var subject = $"Vandivier Product Request Complete: {request.RequestDescription}";
			var emailAddresses = new GraphClient(_configuration, false).GetGroupUsersEmail(_configuration.GetValue<string>("SecurityGroups:HeadQuarters"));

			foreach (var emailAddress in emailAddresses)
			{
				var body = $"Request {request.RequestDescription} has been completed! <br /><br />" +
				           $"View the request <a href='{adminUrl}{requestEditPath}'>here</a> <br /><br />" +
				           $"Thanks, <br /> Vandivier Management";

				if (emailAddress != null)
				{
					Helper.Send(_configuration, subject, body, new List<string>() { emailAddress });
				}

			}
		}

        private void SendRequestCompletedToStoreManager(Request request)
        {
			var adminUrl = _configuration.GetValue<string>("AdminWebsite:BaseUrl");
			var requestEditPath = string.Format(_configuration.GetValue<string>("AdminWebsite:RequestEdit"), request.Id);
			var subject = $"Vandivier Product Request Complete: {request.RequestDescription}";
			var store = _dbReadService.GetSingleRecordAsync<Store>(s => s.Id.Equals(request.StoreId)).Result;
			var email = store.StoreSupervisorEmail;

			var body = $"Request {request.RequestDescription} has been completed! <br /><br />" +
					   $"View the request <a href='{adminUrl}{requestEditPath}'>here</a> <br /><br />" +
			           $"Thanks, <br /> Vandivier Management";

			if (email != null)
			{
				Helper.Send(_configuration, subject, body, new List<string>() { email });
			}
		}

        private void SendRequestCompletedToOriginatingUser(Request request)
        {
	        var adminUrl = _configuration.GetValue<string>("AdminWebsite:BaseUrl");
	        var requestEditPath = string.Format(_configuration.GetValue<string>("AdminWebsite:RequestEdit"), request.Id);
			var subject = $"Vandivier Product Request Complete: {request.RequestDescription}";
			var emailAddress = new GraphClient(_configuration, false).GetUserEmail(request.UserId);
			var body = $"Request {request.RequestDescription} has been completed! <br /><br />" +
					   $"View the request <a href='{adminUrl}{requestEditPath}'>here</a> <br /><br />" +
					   $"Thanks, <br /> Vandivier Management";

			if (emailAddress != null)
			{
				Helper.Send(_configuration, subject, body, new List<string>() { emailAddress });
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

		public void SendRequestToStoreManager(Entity.Models.Request request)
		{
			var adminUrl = _configuration.GetValue<string>("AdminWebsite:BaseUrl");
			var requestEditPath = string.Format(_configuration.GetValue<string>("AdminWebsite:RequestEdit"), request.Id);
			var subject = $"Vandivier Product Request";
			var store = _dbReadService.GetSingleRecordAsync<Store>(s => s.Id.Equals(request.StoreId)).Result;
			var email = store.StoreSupervisorEmail;

			var body = $"A new product request has been added that requires your attention. <br /><br />" +
				           $"View the request <a href='{adminUrl}{requestEditPath}'>here</a> <br /><br />" +
				           $"Thanks, <br /> Vandivier Management";

			if (email != null)
			{
				Helper.Send(_configuration, subject, body, new List<string>() { email });
			}

		}

		public void SendNewNoteEmailToOriginatingUser(Entity.Models.Request request, Entity.Models.Note note, int? supplierId = 0)
        {
			if (supplierId != null && supplierId > 0)
			{
				var vendorUrl = _configuration.GetValue<string>("VendorWebsite:BaseUrl");
				var noteDetailPath = string.Format(_configuration.GetValue<string>("VendorWebsite:NoteDetail"), note.Id);
				var subject = $"Vandivier Product Request Note";

				var emailAddress = new GraphClient(_configuration, false).GetUserEmail(request.UserId);
				var body = $"A new note has been added for a product request that requires your attention. <br /><br />" +
                        $"View the note <a href='{vendorUrl}{noteDetailPath}'>here</a> <br /><br />" +
                        $"Thanks, <br /> Vandivier Management";

				if (emailAddress != null)
				{
					Helper.Send(_configuration, subject, body, new List<string>() { emailAddress });
				}
			}
			else
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
        }

		public async void SendNewNoteEmailToSuppliers(Entity.Models.Request request, Entity.Models.Note note)
        {
            if (request.SupplierId.HasValue == false)
                return;

            var vendorUrl = _configuration.GetValue<string>("VendorWebsite:BaseUrl");
            var noteDetailPath = string.Format(_configuration.GetValue<string>("VendorWebsite:NoteDetail"), note.Id);
            var subject = $"Vandivier Product Request Note";
            var users = _dbReadService.GetAllRecordsAsync<Entity.Models.User>(x => x.SupplierId == request.SupplierId).Result;

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
