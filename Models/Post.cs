//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DTransAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Post
    {
        public Post()
        {
            this.Agent_Post = new HashSet<Agent_Post>();
            this.Notifications = new HashSet<Notification>();
        }
    
        public int PostId { get; set; }
        public Nullable<System.DateTime> DatePosted { get; set; }
        public Nullable<System.TimeSpan> TimePosted { get; set; }
        public int SubscriberId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Weight { get; set; }
        public Nullable<int> Fragility { get; set; }
        public string LocationToId { get; set; }
        public string LocationFromId { get; set; }
        public string PickUpPoint { get; set; }
        public Nullable<double> ProposedFee { get; set; }
        public string DeliveryDate { get; set; }
        public string Status { get; set; }
        public string ParcelPic { get; set; }
        public string AddressTo { get; set; }
        public string AgentID { get; set; }
        public ImageUp upload { get; set; }

        public virtual ICollection<Agent_Post> Agent_Post { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual Subscriber Subscriber { get; set; }
    }
}