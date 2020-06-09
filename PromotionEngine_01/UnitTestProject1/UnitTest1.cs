using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromotionEngine_01;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod_Product_Create()
        {
            List<Product> products;
            List<Product> exp_products = new List<Product>
            {
                new Product('A',50), new Product('B',30),new Product('C',20), new Product('D',15)
            };
            products = Program.LoadProductDetails();
            Assert.AreEqual(exp_products.Count, products.Count);
            for (int i = 0; i < exp_products.Count; i++)
            {
                Assert.AreEqual(exp_products[i].ProductID, products[i].ProductID);
                Assert.AreEqual(exp_products[i].ProductPrice, products[i].ProductPrice);
            }
        }

        [TestMethod]
        public void TestMethod_ProductOffers_Create()
        {
            List<ProductOffers> productOffers;
            List<ProductOffers> exp_productOffers = new List<ProductOffers>
            {
                new ProductOffers(1, 'A', 3, 130, 0),
                new ProductOffers(2, 'B', 2, 45, 0),
                new ProductOffers(3, 'C', 1, 30, 0),
                new ProductOffers(3, 'D', 1, 30, 0),
            };
            productOffers = Program.LoadProductsOffersDetails();
            Assert.AreEqual(exp_productOffers.Count, productOffers.Count);
            for (int i = 0; i < exp_productOffers.Count; i++)
            {
                Assert.AreEqual(exp_productOffers[i].OfferProductID, productOffers[i].OfferProductID);
                Assert.AreEqual(exp_productOffers[i].OfferID, productOffers[i].OfferID);
                Assert.AreEqual(exp_productOffers[i].OfferQuantity, productOffers[i].OfferQuantity);
                Assert.AreEqual(exp_productOffers[i].FixedPriceOffer, productOffers[i].FixedPriceOffer);
                Assert.AreEqual(exp_productOffers[i].DiscountPriceOffer, productOffers[i].DiscountPriceOffer);
            }
        }

        [TestMethod]
        public void TestMethod_UserOrder_Create()
        {
            List<UserOrder> userOrder;
            List<UserOrder> exp_userOrder = new List<UserOrder>();
            //Scenario A
            exp_userOrder.Add(new UserOrder('A', 1));
            exp_userOrder.Add(new UserOrder('B', 1));
            exp_userOrder.Add(new UserOrder('C', 1));

            userOrder = Program.LoadUserOrderDetails(1);
            Assert.AreEqual(exp_userOrder.Count, userOrder.Count);
            for (int i = 0; i < exp_userOrder.Count; i++)
            {
                Assert.AreEqual(exp_userOrder[i].ProductID, userOrder[i].ProductID);
                Assert.AreEqual(exp_userOrder[i].OrgQuantity, userOrder[i].OrgQuantity);
            }
        }

        [TestMethod]
        public void TestMethod_UserOrder_0()
        {
            List<UserOrder> exp_userOrder = new List<UserOrder>();
            exp_userOrder.Add(new UserOrder('A', 1));
            exp_userOrder.Add(new UserOrder('B', 2));

            List<Product> exp_products = new List<Product>
            {
                new Product('A',50), new Product('B',30),new Product('C',20), new Product('D',15)
            };

            exp_userOrder = Program.CheckProductWithoutOffer(exp_userOrder, exp_products);
            Assert.AreEqual(50, exp_userOrder[0].TotalPrice);
            Assert.AreEqual(60, exp_userOrder[1].TotalPrice);
        }

        [TestMethod]
        public void TestMethod_UserOrder_1()
        {
            List<UserOrder> exp_userOrder = new List<UserOrder>();
            exp_userOrder.Add(new UserOrder('A', 0));
            exp_userOrder.Add(new UserOrder('B', 1));

            List<Product> exp_products = new List<Product>
            {
                new Product('A',50), new Product('B',30),new Product('C',20), new Product('D',15)
            };

            exp_userOrder = Program.CheckProductWithoutOffer(exp_userOrder, exp_products);
            Assert.AreEqual(0, exp_userOrder[0].TotalPrice);
            Assert.AreEqual(30, exp_userOrder[1].TotalPrice);
        }

        [TestMethod]
        public void TestMethod_UserOrder_2()
        {
            List<UserOrder> exp_userOrder = new List<UserOrder>();
            exp_userOrder.Add(new UserOrder('A', 2));
            exp_userOrder.Add(new UserOrder('B', 2));

            List<Product> exp_products = new List<Product>
            {
                new Product('A',50), new Product('B',30),new Product('C',20), new Product('D',15)
            };
            List<ProductOffers> exp_productOffers = new List<ProductOffers>
            {
                new ProductOffers(1, 'A', 3, 130, 0),
                new ProductOffers(2, 'B', 2, 45, 0),
            };
            exp_userOrder = Program.CheckProductWithOffer(exp_userOrder, exp_productOffers, exp_products);
            Assert.AreEqual(0, exp_userOrder[0].TotalPrice);
            Assert.AreEqual(45, exp_userOrder[1].TotalPrice);

            Assert.AreEqual(exp_userOrder[0].OrgQuantity, exp_userOrder[0].RemQuantity);
            Assert.AreEqual(0, exp_userOrder[1].RemQuantity);
        }

        [TestMethod]
        public void TestMethod_UserOrder_2_1()
        {
            float totalPrice = 0;
            List<Product> exp_products = new List<Product>
            {
                new Product('A',50), new Product('B',30),new Product('C',20), new Product('D',15)
            };
            totalPrice = Program.updateProductPriceTotal(exp_products, 'A', 20, 2);

            Assert.AreEqual(80, totalPrice);

            totalPrice = Program.updateProductPriceTotal(exp_products, 'B', 25, 3);

            Assert.AreEqual(67.5, totalPrice);
        }

        [TestMethod]
        public void TestMethod_UserOrder_3()
        {
            List<UserOrder> exp_userOrder = new List<UserOrder>();
            float orderTotal = 0;
            exp_userOrder.Add(new UserOrder('A', 4));
            exp_userOrder.Add(new UserOrder('B', 3));

            List<Product> exp_products = new List<Product>
            {
                new Product('A',50), new Product('B',30),new Product('C',20), new Product('D',15)
            };
            List<ProductOffers> exp_productOffers = new List<ProductOffers>
            {
                new ProductOffers(1, 'A', 3, 130, 0),
                new ProductOffers(2, 'B', 2, 45, 0),
            };
            exp_userOrder = Program.CheckProductWithOffer(exp_userOrder, exp_productOffers, exp_products);
            exp_userOrder = Program.CheckProductWithoutOffer(exp_userOrder, exp_products);
            orderTotal = Program.PrintUserOrders(exp_userOrder);

            Assert.AreEqual(180, exp_userOrder[0].TotalPrice);
            Assert.AreEqual(75, exp_userOrder[1].TotalPrice);

            Assert.AreEqual(0, exp_userOrder[0].RemQuantity);
            Assert.AreEqual(0, exp_userOrder[1].RemQuantity);

            Assert.AreEqual(255, orderTotal);
        }
    }
}
