using System.CodeDom;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using mongo;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvc.Models
{
    public class User
    {
        public ObjectId _id { get; set; }
	    public string Name { get; set; }
	    public string Email { get; set; }
	    public string Password { get; set; }
	    public string Salt { get; set; }

        public User()
        {
            
        }

	    public User(string email, string password)
	    {
	        var dbUser = new MongoTable<User>();

	        var user = dbUser.Collection().FindOneAs<User>();
	        if (user != null)
	        {
	            var hashedPassword = Hash.HashPassword(password, user.Salt);
	            if (user.Email == email && user.Password == hashedPassword)
	            {
	                _id = user._id;
	                Name = user.Name;
	                Email = user.Email;
	                Password = hashedPassword;
	                Salt = user.Salt;

                    if(OnUserAuthorized != null)
	                    OnUserAuthorized(email, EventArgs.Empty);

	                return;
	            }
	        }

            if (OnUserUnauthorized != null)
	            OnUserUnauthorized(email, EventArgs.Empty);

            throw new UnauthorizedException("username or password is wrong");
	    }

	    public static event EventHandler OnUserAuthorized;
	    public static event EventHandler OnUserUnauthorized;


	    public void ResetPassword(string password)
	    {
		    // TODO
	    }
    }

    [Serializable]
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
        {
        }

        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnauthorizedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}