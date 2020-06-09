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
    public class Product
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
    public class ProductOffers
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
    public class UserOrder
    {
        public char ProductID { get; set; }
        public int OrgQuantity { get; set; }
        public float TotalPrice { get; set; }
        public int RemQuantity { get; set; }
        public string offerDetails { get; set; }
        public UserOrder(char PID, int Qty)
        {
            ProductID = PID;
            OrgQuantity = Qty;
            RemQuantity = OrgQuantity;
            offerDetails = string.Empty;
        }
    }

    public class Program
    {

        static void Main(string[] args)
        {
            List<Product> products;
            List<ProductOffers> productOffers;
            List<UserOrder> userOrder;
            float TotalPrice = 0;

            // Load Product Details
            products = LoadProductDetails();
            // Load Product Offers Detail
            productOffers = LoadProductsOffersDetails();
            // Load User Order Details
            userOrder = LoadUserOrderDetails(3);
            // Checkout Product with Offer Details
            userOrder = CheckProductWithOffer(userOrder, productOffers, products);
            // Checkout Product without any offers
            userOrder = CheckProductWithoutOffer(userOrder, products);
            // Print User order Details
            TotalPrice = PrintUserOrders(userOrder);
            Console.WriteLine("Total {0}", TotalPrice.ToString());
            Console.ReadLine();
        }

        #region Load
        public static List<UserOrder> LoadUserOrderDetails(int Scenario)
        {
            List<UserOrder> userOrder = new List<UserOrder>();
            switch(Scenario)
            {
                case 1:
                    //Scenario A
                    userOrder.Add(new UserOrder('A', 1));
                    userOrder.Add(new UserOrder('B', 1));
                    userOrder.Add(new UserOrder('C', 1));
                    break;
                case 2:
                    //Scenario B
                    userOrder.Add(new UserOrder('A', 5));
                    userOrder.Add(new UserOrder('B', 5));
                    userOrder.Add(new UserOrder('C', 1));
                    break;
                case 3:
                    //Scenario C
                    userOrder.Add(new UserOrder('A', 3));
                    userOrder.Add(new UserOrder('B', 5));
                    userOrder.Add(new UserOrder('C', 1));
                    userOrder.Add(new UserOrder('D', 1));
                    break;
                default:
                    break;
            }

            return userOrder;
        }

        public static List<ProductOffers> LoadProductsOffersDetails()
        {
            List<ProductOffers> productOffers = new List<ProductOffers>
            {
                new ProductOffers(1, 'A', 3, 130, 0),
                new ProductOffers(2, 'B', 2, 45, 0),
                new ProductOffers(3, 'C', 1, 30, 0),
                new ProductOffers(3, 'D', 1, 30, 0),

            };
            return productOffers;
        }
        public static List<Product> LoadProductDetails()
        {
            List<Product> products = new List<Product>
            {
                new Product('A',50), new Product('B',30),new Product('C',20), new Product('D',15)
            };
            return products;
        }
        #endregion

        #region Checkout User orders
        /// <summary>
        /// User orders without offers
        /// </summary>
        public static List<UserOrder> CheckProductWithoutOffer(List<UserOrder> userOrder, List<Product> products)
        {
            foreach (var order in userOrder)
            {
                if (order.RemQuantity > 0)
                {
                    float baseProductPrice = products.Where(s => s.ProductID == order.ProductID).Select(x => x.ProductPrice).FirstOrDefault();
                    order.TotalPrice += (baseProductPrice * order.RemQuantity);
                    order.offerDetails += string.Format("{0}*{1}+", order.RemQuantity, baseProductPrice);
                    order.RemQuantity--;
                }
                order.offerDetails = order.offerDetails.TrimEnd('+');
            }
            return userOrder;
        }

        /// <summary>
        /// User orders with offers
        /// </summary>
        public static List<UserOrder> CheckProductWithOffer(List<UserOrder> userOrder, List<ProductOffers> productOffers, List<Product> products)
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

                            filteredOrder.offerDetails += string.Format("{0}+", offer.FixedPriceOffer > 0 ? offer.FixedPriceOffer :
                                (filteredOrder.TotalPrice - (filteredOrder.TotalPrice * (offer.DiscountPriceOffer / 100))));
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
                                                a.DiscountPriceOffer,
                                                d.offerDetails
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
                                    {
                                        float total = offer.FixedPriceOffer > 0 ? offer.FixedPriceOffer : updateProductPriceTotal(products, order.ProductID, offer.DiscountPriceOffer, item.OfferQuantity);
                                        order.TotalPrice += total;
                                        order.offerDetails += string.Format("{0}", total);
                                    }
                                    order.RemQuantity -= item.OfferQuantity;
                                }
                            }
                            sameOffer = true;
                        }
                        //order.offerDetails = order.offerDetails.TrimEnd('+')  + string.Format(")");
                    }
                }
            }
            return userOrder;
        }


        #endregion

        /// <summary>
        /// Discount price calculation
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="discountPriceOffer"></param>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public static float updateProductPriceTotal(List<Product> products, char productId, float discountPriceOffer, int Quantity)
        {
            float totalProductPrice = 0;
            float baseProductPrice = products.Where(s => s.ProductID == productId).Select(x => x.ProductPrice).FirstOrDefault();

            totalProductPrice = (baseProductPrice * (1 - (discountPriceOffer / 100))) * Quantity;
            return totalProductPrice;
        }

        /// <summary>
        /// Print User Order details
        /// </summary>
        public static float PrintUserOrders(List<UserOrder> userOrder)
        {
            float OrderSum = 0;
            foreach (var order in userOrder)
            {
                string outupt = String.Format("{0} * {1} = {2} ({3})", order.ProductID.ToString(), order.OrgQuantity.ToString(), order.TotalPrice.ToString(), order.offerDetails);
                Console.WriteLine(outupt);
                OrderSum += order.TotalPrice;
            }
            return OrderSum;
        }
    }
}
