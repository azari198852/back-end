using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class SellerCommentDto
    {

        public long Id { get; set; }
        public long? SellerId { get; set; }
        public  string SellerName { get; set; }
        public string CommentType { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public string ProfileImage { get; set; }

    }
}
