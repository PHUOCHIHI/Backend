using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Login
{
    [Serializable]
    public class RequestLoginData
    {
        public string email;
        public string password;
        public string twoFactorCode = "";
        public string twoFactorRecoveryCode = "";

        public RequestLoginData(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }

    [Serializable]
    public class ResponseLogin
    {
        public bool isSuccess;
        public string notification;
        public LoginUserData data;
    }

    [Serializable]
    public class LoginUserData
    {
        public string token;
        public User user;
    }

    [Serializable]
    public class User
    {
        public string id;
        public string email;
        public string name;
        public string avatar;
        public int regionId;
    }

    [Serializable]
    public class ForgotPasswordRequest
    {
        public string email;

        public ForgotPasswordRequest(string email)
        {
            this.email = email;
        }
    }

    [Serializable]
    public class CheckOtpRequest
    {
        public string email;
        public string otp;

        public CheckOtpRequest(string email, string otp)
        {
            this.email = email;
            this.otp = otp;
        }
    }

    [Serializable]
    public class ResetPasswordRequest
    {
        public string email;
        public string otp;
        public string newPassword;

        public ResetPasswordRequest(string email, string otp, string newPassword)
        {
            this.email = email;
            this.otp = otp;
            this.newPassword = newPassword;
        }
    }
}
