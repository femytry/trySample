using System;
using System.ComponentModel.DataAnnotations;

namespace trySample.Entities
{
    public class User
    {
        public Guid Id{ get; set;}
        [StringLength(15)]
        public string Username{ get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Token{ get; set; }
    }
}