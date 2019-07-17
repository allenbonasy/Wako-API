using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.Entity;
using System.Web;
using DTransAPI.Models;
using System.Text;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Threading;
using System.Net.Http.Headers;

namespace DTransAPI.Controllers
{


    public class SubscriptionsController : ApiController
    {


        DtransEntities db = new DtransEntities();
        static String reff, pollURL;

        private string GenerateTwoWayHash(Dictionary<string, string> items, string guid)
        {

            string concat = string.Join("", items.Select(c => (c.Value != null ? c.Value.Trim() : "")).ToArray());
            System.Security.Cryptography.SHA512 check = System.Security.Cryptography.SHA512.Create();
            byte[] resultArr = check.ComputeHash(System.Text.Encoding.UTF8.GetBytes(concat + guid));
            return ByteArrayToString(resultArr);
        }


        private string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }


        [Route("api/Paynow")]
        public String PostCreate(Sub_Post pos)
        {
            int postID = Int32.Parse(pos.PostID);
            int payeeID = Int32.Parse(pos.payeeID);
            var post = db.Posts.Find(postID);
            var payee = db.Agents.Find(payeeID);
            var payer = post.Subscriber;

            String papers = post.PostId + ":[" + "To:" + payee.SubscriberId + "_From:" + payer.SubscriberId + "]";
            Double price = 0;
            String ResultXml = "";

            price = (Double)post.ProposedFee;
            string payNowMerchantKey = db.Settings.Where(e => e.VariableName == "payNowMerchantKey").Select(e => e.VariableValue).First();
            string payNowId = db.Settings.Where(e => e.VariableName == "payNowId").Select(e => e.VariableValue).First();
            string payNowResultUrl = db.Settings.Where(e => e.VariableName == "payNowResultUrl").Select(e => e.VariableValue).First();
            string payNowReturnUrl = db.Settings.Where(e => e.VariableName == "payNowReturnUrl").Select(e => e.VariableValue).First();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            string resultUrl = payNowResultUrl;
            string returnUrl = payNowReturnUrl;
            string reference = papers;

            string amount = price.ToString();


            string authemail = payer.Email;
            string merchant_key = payNowMerchantKey;

            //put them in dictionary
            dic.Add("resulturl", resultUrl);
            dic.Add("returnurl", returnUrl);
            dic.Add("reference", reference);
            dic.Add("amount", amount);
            dic.Add("id", payNowId);
            dic.Add("additionalinfo", "Anything");
            dic.Add("authemail", authemail);
            dic.Add("status", "message");


            Dictionary<string, object> finalDic = new Dictionary<string, object>();

            using (WebClient client = new WebClient())
            {
                string hash = GenerateTwoWayHash(dic, merchant_key).ToUpper();

                NameValueCollection newColl = new NameValueCollection();
                foreach (var pair in dic)
                {
                    newColl.Add(pair.Key, pair.Value);
                }

                newColl.Add("hash", hash);

                //prepare another dictionary
                byte[] response = client.UploadValues("https://www.paynow.co.zw/interface/initiatetransaction", newColl);

                string result = System.Text.Encoding.UTF8.GetString(response);
                ResultXml = result;


                string browseUrl = null;
                string pollUrl = null;
                string stat = null;
                string hashed = null;
                string[] temp = result.Split(new char[] { '&' });
                foreach (String str in temp)
                {
                    if (str.StartsWith("status"))
                        stat = str.Split(new char[] { '=' })[1];
                    if (str.StartsWith("browserurl"))
                        browseUrl = str.Split(new char[] { '=' })[1];
                    if (str.StartsWith("pollurl"))
                        pollUrl = str.Split(new char[] { '=' })[1];
                    if (str.StartsWith("hash"))
                        hashed = str.Split(new char[] { '=' })[1];

                }
                pollURL = pollUrl.Replace("%3a", ":").Replace("%2f", "/").Replace("%3f", "?").Replace("%3d", "=");

                browseUrl = browseUrl.Replace("%3a", ":").Replace("%2f", "/");

                browseUrl = browseUrl.Replace("%3a", ":").Replace("%2f", "/");
                if (stat == "Ok")
                {
                    reff = reference;
                    if (db.TransactionFees.FirstOrDefault(ss => ss.reff_Number == reference) != null)
                    {
                        TransactionFee tr = db.TransactionFees.FirstOrDefault(ss => ss.reff_Number == reference);
                        tr.Status = "Payment Initiated";
                        tr.Date = DateTime.Now.Date;
                        tr.Time = DateTime.Now.TimeOfDay;
                        tr.PollURL = pollURL;
                        tr.Hash = hash;
                        tr.reff_Number = reff;
                        db.Entry(tr).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else {

                        TransactionFee trans = new TransactionFee
                        {
                            AgentID = payee.AgentId,
                            amount = price,
                            Date = DateTime.Now.Date,
                            postID = post.PostId,
                            reff_Number = reference,
                            Time = DateTime.Now.TimeOfDay,
                            Status = stat,
                            Submitted = "Awaiting",
                            PollURL = pollURL,
                            Hash = hash
                        };
                        db.TransactionFees.Add(trans);
                        db.SaveChanges();

                    }

                    return browseUrl.ToString();

                }
                else
                {
                    if (db.TransactionFees.FirstOrDefault(ss => ss.reff_Number == reference) != null)
                    {
                        TransactionFee tr = db.TransactionFees.FirstOrDefault(ss => ss.reff_Number == reference);
                        tr.Status = stat;
                        tr.Date = DateTime.Now.Date;
                        tr.Time = DateTime.Now.TimeOfDay;
                        db.Entry(tr).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        TransactionFee trans = new TransactionFee
                        {
                            AgentID = payee.SubscriberId,
                            amount = price,
                            Date = DateTime.Now.Date,
                            postID = post.PostId,
                            reff_Number = reference,
                            Time = DateTime.Now.TimeOfDay,
                            Status = "Payment Failed"
                        };
                        db.TransactionFees.Add(trans);
                        db.SaveChanges();
                    }
                    string results = System.IO.File.ReadAllText(HttpContext.Current.Request.MapPath("/ProPics/index.txt"));
                    return results.Replace("Status", "Payment Status : Failed.");
                }
            }








        }
        [Route("api/payGet")]

           public HttpResponseMessage GetResult()
        {


            string url = pollURL;
            Subscriber user = new Subscriber();
            string URI = url;
            string stat = "Failed Please Try Again";
            string[] arr = url.Split(char.Parse("?"));
            string urWeb = arr[0];
            string guid = arr[1];
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";



                string HtmlResult = wc.UploadString(URI, "POST", guid);


                DateTime d = new DateTime(1985, 03, 03);
                DateTime utc = DateTime.SpecifyKind(d, DateTimeKind.Utc);



                string[] temp = HtmlResult.Split(new char[] { '&' });
                string hash = "";
                string paynowreference = "";
                foreach (String str in temp)
                {
                    if (str.StartsWith("status"))
                        stat = str.Split(new char[] { '=' })[1];
                    if (str.StartsWith("reference"))
                        reff = str.Split(new char[] { '=' })[1];
                    if (str.StartsWith("pollurl"))
                        pollURL = str.Split(new char[] { '=' })[1];
                    if (str.StartsWith("pollurl"))
                        hash = str.Split(new char[] { '=' })[1];

                    if (str.StartsWith("paynowreference"))
                        paynowreference = str.Split(new char[] { '=' })[1];

                }

                reff = reff.Replace("%3a", ":").Replace("%2f", "/").Replace("%3f", "?").Replace("%3d", "=").Replace("%5d", "]").Replace("%5b", "[");
                if (stat == "Paid")

                {
                    hash = hash.Replace("%3a", ":").Replace("%2f", "/").Replace("%3f", "?").Replace("%3d", "=");
                    //Update Transaction
                    TransactionFee tr = db.TransactionFees.FirstOrDefault(ss => ss.reff_Number == reff);
                    tr.Status = stat;
                    tr.Date = DateTime.Now.Date;
                    tr.Time = DateTime.Now.TimeOfDay;
                    tr.Hash = hash;
                    tr.reff_Number = reff;
                    tr.paymentReff = paynowreference;
                    db.Entry(tr).State = EntityState.Modified;
                    //
                    reff = reff.Replace(":", " ").Replace("]", "").Replace("[", "").Replace("From", "").Replace("To", "").Replace("_", "");
                    String[] refer = reff.Split(' ');
                    int id = Int32.Parse(refer[0]);
                    Agent a = db.Agents.Find(tr.AgentID);
                    Post post = db.Posts.Find(id);
                    post.Status = "Paid.Waiting for dellivery . Reff Number=" + tr.transID;
                    db.Entry(post).State = EntityState.Modified;

                    Agent_Post apost = new Agent_Post
                    {
                        AgentId = tr.AgentID,
                        Date = tr.Date,
                        Eng_PostId = id,
                        Poster_Conf = "Waiting for delivery",
                        Rec_Date = null,
                        Status = "Waiting for Delivery"
                    };
                    db.Agent_Post.Add(apost);

                    db.SaveChanges();
                    PostEmail(a.Subscriber, post);


                    var response = new HttpResponseMessage();
                    String results = System.IO.File.ReadAllText(HttpContext.Current.Request.MapPath("/ProPics/index.txt"));
                    response.Content = new StringContent(results.Replace("Status", "Payment Status : " + stat));
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                    return response;


                }
                else
                {
                    TransactionFee tr = db.TransactionFees.FirstOrDefault(ss => ss.reff_Number == reff);
                    tr.Status = stat;
                    tr.Date = DateTime.Now.Date;
                    tr.Time = DateTime.Now.TimeOfDay;
                    tr.Hash = hash;
                    tr.PollURL = pollURL;
                    tr.paymentReff = paynowreference;
                    db.Entry(tr).State = EntityState.Modified;

                    reff = reff.Replace(":", " ").Replace("]", "").Replace("[", "").Replace("From", "").Replace("To", "").Replace("_", "");
                    String[] refer = reff.Split(' ');
                    int id = Int32.Parse(refer[0]);
                    //int agntID = Int32.Parse(refer[3]);
                    //Update Posts
                    Post post = db.Posts.Find(id);
                    post.Status = "Pending";
                    db.Entry(post).State = EntityState.Modified;
                    db.SaveChanges();
                    var response = new HttpResponseMessage();
                    String results = System.IO.File.ReadAllText(HttpContext.Current.Request.MapPath("/ProPics/index.txt"));
                    response.Content = new StringContent(results.Replace("Status", "Payment Status : "+stat));
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                    return response;
                 
                }


            }


        }



        [Route("api/payResult")]
        public void PostResult([FromBody]string obj)
        {
            //Poll for sucess
            string url = obj;
            Subscriber user = new Subscriber();
            string URI = url;
            string stat = null;
            string[] arr = url.Split(char.Parse("?"));
            string urWeb = arr[0];
            string guid = arr[1];
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";



                string HtmlResult = wc.UploadString(URI, "POST", guid);


                DateTime d = new DateTime(1985, 03, 03);
                DateTime utc = DateTime.SpecifyKind(d, DateTimeKind.Utc);



                string[] temp = HtmlResult.Split(new char[] { '&' });
                string hash = "";
                string paynowreference = "";
                foreach (String str in temp)
                {
                    if (str.StartsWith("status"))
                        stat = str.Split(new char[] { '=' })[1];
                    if (str.StartsWith("reference"))
                        reff = str.Split(new char[] { '=' })[1];
                    if (str.StartsWith("pollurl"))
                        pollURL = str.Split(new char[] { '=' })[1];
                    if (str.StartsWith("pollurl"))
                        hash = str.Split(new char[] { '=' })[1];

                    if (str.StartsWith("paynowreference"))
                        paynowreference = str.Split(new char[] { '=' })[1];

                }

                if (stat == "Paid")
                {

                    TransactionFee tr = db.TransactionFees.FirstOrDefault(ss => ss.reff_Number == reff && ss.Hash == hash);
                    tr.Status = stat;
                    tr.Date = DateTime.Now.Date;
                    tr.Time = DateTime.Now.TimeOfDay;
                    tr.Hash = hash;
                    tr.PollURL = pollURL;
                    tr.paymentReff = paynowreference;
                    db.Entry(tr).State = EntityState.Modified;
                    db.SaveChanges();

                    Post post = db.Posts.Find(tr.postID);
                    if (post != null) {
                        post.Status = "Paid. Waiting for dellivery . Reff Number=" + tr.transID;
                        db.Entry(post).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                   
                 
                    PostEmail(post.Subscriber, post);
                }
                else
                {
                    TransactionFee tr = db.TransactionFees.FirstOrDefault(ss => ss.reff_Number == reff);
                    tr.Status = stat;
                    tr.Date = DateTime.Now.Date;
                    tr.Time = DateTime.Now.TimeOfDay;
                    tr.Hash = hash;
                    tr.PollURL = pollURL;
                    tr.paymentReff = paynowreference;
                    db.Entry(tr).State = EntityState.Modified;
                    db.SaveChanges();


                }


            }


        }

        [Route("api/Verfy")]
        public String PostEmail(Subscriber sub,Post post)
        {
            string check = "";

            var message = new MailMessage();

           message.To.Add(new MailAddress(sub.Email));
            
             //message.To.Add(new MailAddress("allenbonasy@gmail.com"));
            // replace with valid value 
            message.From = new MailAddress("wakho.notifications@gmail.com","Wakho");  // replace with valid value
            message.Subject = "Payment Confirmation";

            var body = "<div><p> Hello " + sub.Name + " " + sub.Surname + "</p><br /><p>Your parcel : " + post.Title + ". " + " has been paid for by Poster .Deliver   the parcel before " + post.DeliveryDate+ ", You will get your full payement after the Poster has confirmed Item Delivery.</p></div>"+
               " <p>  Payment will be made at the payement details you provided .NOTE adjustments to the bank details provided can only be done before the Poster has received the parcel  . </p><p>Thank you</p>";
            //message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
            message.Body = string.Format(body);

            message.IsBodyHtml = true;
            try
            {



                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        // UserName = "zimpapersdev@gmail.com",  // replace with valid value
                        //Password = "NETgrpsvr2012dcc"  // replace with valid value
                        UserName = "wakho.notifications@gmail.com",
                        Password = "tongayisithole"
                        //Password = pass, // replace with valid value
                        //UserName = email,

                    };
                    smtp.Credentials = credential;
                    if (sub.Email.Contains("@outlook.com"))
                    {
                        smtp.Host = "smtp-mail.outlook.com";
                        smtp.Port = 587;
                    }
                    else if (sub.Email.Contains("@yahoo.com"))
                    {
                        smtp.Host = "smtp.mail.yahoo.com";
                        smtp.Port = 465;
                    }
                    else if (sub.Email.Contains("@gmail.com"))
                    {
                        smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;

                    smtp.EnableSsl = true;
                     }



                    smtp.Send(message);
                    check = sub.VerificationCode;

                }
            }
            catch (Exception e)
            {

                check = e.Message;
            }

            return check;
        }


       
    }
}
