using InstamojoAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace CloudSociety.Services
{
    public class PaymentService
    {
        private string Insta_client_id, Insta_client_secret, Insta_Endpoint, Insta_Auth_Endpoint;
        private Instamojo objClass;
        public PaymentService(String clientId, String clientSecret, String apiEndpoint = "", String authEndpoint = "")
        {
            Insta_client_id = clientId;
            Insta_client_secret = clientSecret;
            Insta_Endpoint = apiEndpoint == "" ? InstamojoConstants.INSTAMOJO_API_ENDPOINT : apiEndpoint;
            Insta_Auth_Endpoint = authEndpoint == "" ? InstamojoConstants.INSTAMOJO_AUTH_ENDPOINT : authEndpoint;
            objClass = InstamojoImplementation.getApi(Insta_client_id, Insta_client_secret, Insta_Endpoint, Insta_Auth_Endpoint);
        }
        public CreatePaymentOrderResponse CreatePaymentOrder(PaymentOrder objPaymentRequest)
        {
            CreatePaymentOrderResponse objPaymentResponse = null;
            if (null == objPaymentRequest)
            {
                return objPaymentResponse;
            }
            try
            {
                # region   1. Create Payment Order
                //Create Payment Order
                //PaymentOrder objPaymentRequest = new PaymentOrder();
                //Required POST parameters
                //objPaymentRequest.name = "ABCD";
                //objPaymentRequest.email = "foo@example.com";
                //objPaymentRequest.phone = "9969156561";
                //objPaymentRequest.amount = 9;
                //objPaymentRequest.currency = "Unsupported";

                string randomName = Path.GetRandomFileName();
                randomName = randomName.Replace(".", string.Empty);
                objPaymentRequest.transaction_id = Guid.NewGuid().ToString();

                //objPaymentRequest.redirect_url = "https://swaggerhub.com/api/saich/pay-with-instamojo/1.0.0";
                //Extra POST parameters 



                if (objPaymentRequest.validate())
                {
                    //if (objPaymentRequest.nameInvalid)
                    //{
                    //    //MessageBox.Show("Name is not valid");
                    //    return;
                    //}
                    return objPaymentResponse;
                }
                else
                {
                    try
                    {
                        objPaymentResponse = objClass.createNewPaymentRequest(objPaymentRequest);
                        //MessageBox.Show("Order Id = " + objPaymentResponse.order.id);
                    }
                    catch (ArgumentNullException ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                    catch (WebException ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                    catch (IOException ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                    catch (InvalidPaymentOrderException ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                    catch (ConnectionException ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                    catch (BaseException ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("Error:" + ex.Message);
                    }
                }
                #endregion
            }
            catch (BaseException ex)
            {
                //MessageBox.Show("CustomException" + ex.Message);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Exception" + ex.Message);
            }
            return objPaymentResponse;
        }

        public PaymentOrderDetailsResponse GetDetailsOfPaymentOrder(string TransactionId)
        {
            PaymentOrderDetailsResponse objPaymentRequestDetailsResponse = null;
            # region   2. Get details of this payment order Using TransactionId
            //  Get details of this payment order Using TransactionId
            try
            {
                objPaymentRequestDetailsResponse = objClass.getPaymentOrderDetailsByTransactionId(TransactionId);
            }
            catch (Exception ex)
            {
            }
            #endregion
            return objPaymentRequestDetailsResponse;
        }
    }
}