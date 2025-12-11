using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Register
{
    [Serializable]
    public class RegisterRequestData
    {
        public string email;
        public string password;
        public string name;
        public string linkAvatar;
        public int regionId;

        public RegisterRequestData(string email, string password, string name, string linkAvatar, int regionId)
        {
            this.email = email;
            this.password = password;
            this.name = name;
            this.linkAvatar = linkAvatar;
            this.regionId = regionId;
        }
    }

    [Serializable]
    public class ResponseUserSuccess
    {
        public bool isSuccess;
        public string notifition;
        public RegisterUserData data;
    }

    [Serializable]
    public class RegisterUserData
    {
        public string name;
    }

    [Serializable]
    public class ResponseUserError
    {
        public bool isSuccess;
        public string notifition;
        public List<Error> data;
    }

    [Serializable]
    public class Error
    {
        public string description;
    }
}
