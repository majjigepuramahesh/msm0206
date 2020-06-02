using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionEngine_01
{
    /// <summary>
    /// Product Class
    /// </summary>
    class Product
    {
        public char ProductID { get; set; }
        public float ProductPrice { get; set; }

        public Product(char PID, float price)
        {
            ProductID = PID;
            ProductPrice = price;
        }
    }

    /// <summary>
    /// Product Offer Class
    /// </summary>
    class ProductOffers
    {
        public int OfferID { get; set; }
        public char OfferProductID { get; set; }
        public int OfferQuantity { get; set; }
        public float FixedPriceOffer { get; set; }
        public float DiscountPriceOffer { get; set; }

        public ProductOffers(int OID, char PID, int QTY, float fixedPrice, float discountOffer)
        {
            OfferID = OID;
            OfferProductID = PID;
            OfferQuantity = QTY;
            FixedPriceOffer = fixedPrice;
            DiscountPriceOffer = discountOffer;
        }
    }

    /// <summary>
    /// User Order Details
    /// </summary>
    class UserOrder
    {
        public char ProductID { get; set; }
        public int OrgQuantity { get; set; }
        public float TotalPrice { get; set; }
        public int RemQuantity { get; set; }
        public UserOrder(char PID, int Qty)
        {
            ProductID = PID;
            OrgQuantity = Qty;
            RemQuantity = OrgQuantity;
        }
    }

    class Program
    {
        public static List<Product> products;
        public static List<ProductOffers> productOffers;
        public static List<UserOrder> userOrder;
        static void Main(string[] args)
        {

            // Load Product Details
            LoadProductDetails();
            // Load Product Offers Detail
            LoadProductsOffersDetails();
            // Load User Order Details
            LoadUserOrderDetails();
            // Checkout Product with Offer Details
            CheckProductWithOffer();
            // Checkout Product without any offers
            CheckProductWithoutOffer();
            // Print User order Details
            PrintUserOrders();
            Console.ReadLine();
        }

        #region Load
        private static void LoadUserOrderDetails()
        {
            userOrder = new List<UserOrder>();

            ////Scenario A
            //userOrder.Add(new UserOrder('A', 1));
            //userOrder.Add(new UserOrder('B', 1));
            //userOrder.Add(new UserOrder('C', 1));

            ////Scenario B
            //userOrder.Add(new UserOrder('A', 5));
            //userOrder.Add(new UserOrder('B', 5));
            //userOrder.Add(new UserOrder('C', 1));

            ////Scenario C
            userOrder.Add(new UserOrder('A', 3));
            userOrder.Add(new UserOrder('B', 5));
            userOrder.Add(new UserOrder('C', 1));
            userOrder.Add(new UserOrder('D', 1));
        }

        private static void LoadProductsOffersDetails()
        {
            productOffers = new List<ProductOffers>
            {
                new ProductOffers(1, 'A', 3, 130, 0),
                new ProductOffers(2, 'B', 2, 45, 0),
                new ProductOffers(3, 'C', 1, 30, 0),
                new ProductOffers(3, 'D', 1, 30, 0),

            };
        }
        private static void LoadProductDetails()
        {
            products = new List<Product>
            {
                new Product('A',50), new Product('B',30),new Product('C',20), new Product('D',15)
            };
        }
        #endregion

        #region Checkout User orders
        /// <summary>
        /// User orders without offers
        /// </summary>
        private static void CheckProductWithoutOffer()
        {
            foreach (var order in userOrder)
            {
                float baseProductPrice = products.Where(s => s.ProductID == order.ProductID).Select(x => x.ProductPrice).FirstOrDefault();
                order.TotalPrice += (baseProductPrice * order.RemQuantity);
            }
        }

        /// <summary>
        /// User orders with offers
        /// </summary>
        private static void CheckProductWithOffer()
        {
            var queryProductsByOffer = from pOffer in productOffers
                                       group pOffer by pOffer.OfferID into offerGroup
                                       orderby offerGroup.Key
                                       select offerGroup;


            foreach (var offerGroup in queryProductsByOffer)
            {
                var filteredOfferCount = (from s in offerGroup
                                      where s.OfferID == offerGroup.Key
                                      select s.OfferProductID).Count();
                var filteredOffer = (from s in offerGroup
                                           where s.OfferID == offerGroup.Key
                                           select s);

                foreach (var offer in offerGroup)
                {
                    if (filteredOfferCount == 1)
                    {
                        var filteredOrder = userOrder.Where(s => s.ProductID == offer.OfferProductID && s.OrgQuantity >= offer.OfferQuantity && s.RemQuantity > 0).FirstOrDefault();
                        while (filteredOrder != null && (filteredOrder.RemQuantity - offer.OfferQuantity >= 0))
                        {
                            filteredOrder.TotalPrice += offer.FixedPriceOffer > 0 ? offer.FixedPriceOffer :
                                (filteredOrder.TotalPrice - (filteredOrder.TotalPrice * (offer.DiscountPriceOffer / 100)));
                            filteredOrder.RemQuantity -= offer.OfferQuantity;
                        }
                    }
                    else if (filteredOfferCount > 1)
                    {
                        int loopCount = 0;
                        bool sameOffer = false;
                        var resultArrary = (from d in userOrder
                                            join a in filteredOffer on d.ProductID equals a.OfferProductID
                                            where a.OfferID == offer.OfferID && d.OrgQuantity >= a.OfferQuantity
                                            && d.RemQuantity >= offer.OfferQuantity && d.RemQuantity > 0
                                            select new
                                            {
                                                d.ProductID,
                                                d.OrgQuantity,
                                                d.RemQuantity,
                                                d.TotalPrice,
                                                a.OfferProductID,
                                                a.OfferQuantity,
                                                a.FixedPriceOffer,
                                                a.DiscountPriceOffer
                                            }).ToArray();


                        foreach (var item in resultArrary)
                        {
                            int minCount = resultArrary.Min(s => s.RemQuantity);
                            loopCount = minCount / item.OfferQuantity;
                            var filteredOrder = userOrder.Where(s => s.ProductID == item.OfferProductID && s.OrgQuantity >= offer.OfferQuantity
                            && s.RemQuantity > 0);

                            foreach (var order in filteredOrder)
                            {

                                while (loopCount > 0 && resultArrary != null && filteredOrder != null && resultArrary.Count() == filteredOfferCount &&
                                    order.RemQuantity > 0 && ((order.RemQuantity - item.OfferQuantity) >= 0))
                                {
                                    loopCount--;
                                    if (!sameOffer || offer.DiscountPriceOffer > 0)
                                        order.TotalPrice += offer.FixedPriceOffer > 0 ? offer.FixedPriceOffer : updateProductPriceTotal(order.ProductID, offer.DiscountPriceOffer, item.OfferQuantity); 
                                    order.RemQuantity -= item.OfferQuantity;
                                }
                            }
                            sameOffer = true;
                        }
                    }
                }
            }
        }


        #endregion

        /// <summary>
        /// Discount price calculation
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="discountPriceOffer"></param>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        private static float updateProductPriceTotal(int productId, float discountPriceOffer, int Quantity)
        {
            float totalProductPrice = 0;
            float baseProductPrice = products.Where(s => s.ProductID == productId).Select(x => x.ProductPrice).FirstOrDefault();

            totalProductPrice = (baseProductPrice * (1 - (discountPriceOffer / 100))) * Quantity;
            return totalProductPrice;
        }

        /// <summary>
        /// Print User Order details
        /// </summary>
        private static void PrintUserOrders()
        {
            float OrderSum = 0;
            foreach (var order in userOrder)
            {
                string outupt = String.Format("{0} * {1} = {2}", order.ProductID.ToString(), order.OrgQuantity.ToString(), order.TotalPrice.ToString());
                Console.WriteLine(outupt);
                OrderSum += order.TotalPrice;
            }
            Console.WriteLine("Total {0}", OrderSum.ToString());
        }
    }
}
