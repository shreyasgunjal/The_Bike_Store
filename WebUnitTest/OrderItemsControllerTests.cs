using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;
using WebAPI.Models;

namespace WebAPI.Tests.Controllers
{
    [TestClass]
    public class OrderItemsControllerTests
    {
        private Mock<BikeStores_Team3Entities> _mockDbContext;
        private Mock<DbSet<order_items>> _mockOrderItemsSet;
        private orderitemsController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the mock context and controller
            _mockDbContext = new Mock<BikeStores_Team3Entities>();
            _mockOrderItemsSet = new Mock<DbSet<order_items>>();

            _controller = new orderitemsController
            {
                db = _mockDbContext.Object
            };
        }

        [TestMethod]
        public void Getorder_items_ReturnsAllOrderItems()
        {
            // Arrange
            var mockOrderItems = new List<order_items>
            {
                new order_items { order_id = 1, item_id = 1, product_id = 20, quantity = 5, list_price = 500, discount = 0.5m },
                new order_items { order_id = 1, item_id = 2, product_id = 8, quantity = 2, list_price = 1799, discount = 0.07m }
            }.AsQueryable();

            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.Provider).Returns(mockOrderItems.Provider);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.Expression).Returns(mockOrderItems.Expression);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.ElementType).Returns(mockOrderItems.ElementType);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.GetEnumerator()).Returns(mockOrderItems.GetEnumerator());

            _mockDbContext.Setup(db => db.order_items).Returns(_mockOrderItemsSet.Object);

            // Act
            var result = _controller.Getorder_items() as IEnumerable<order_items>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void Getcategory_ReturnsOrderItemsByOrderId()
        {
            // Arrange
            var mockOrderItems = new List<order_items>
            {
                new order_items { order_id = 1, item_id = 1, product_id=20, quantity = 5, list_price = 500, discount = 0.5m }
            }.AsQueryable();

            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.Provider).Returns(mockOrderItems.Provider);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.Expression).Returns(mockOrderItems.Expression);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.ElementType).Returns(mockOrderItems.ElementType);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.GetEnumerator()).Returns(mockOrderItems.GetEnumerator());

            _mockDbContext.Setup(db => db.order_items).Returns(_mockOrderItemsSet.Object);

            // Act
            var result = _controller.Getorderitem(1) as OkNegotiatedContentResult<List<order_items>>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.Count);
        }

        [TestMethod]
        public void GetExactItem_ReturnsOrderItemByOrderIdAndItemId()
        {
            // Arrange
            var mockOrderItems = new List<order_items>
            {
                new order_items { order_id = 1, item_id = 1, product_id=20, quantity = 5, list_price = 500, discount = 0.5m }
            }.AsQueryable();

            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.Provider).Returns(mockOrderItems.Provider);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.Expression).Returns(mockOrderItems.Expression);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.ElementType).Returns(mockOrderItems.ElementType);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.GetEnumerator()).Returns(mockOrderItems.GetEnumerator());

            _mockDbContext.Setup(db => db.order_items).Returns(_mockOrderItemsSet.Object);

            // Act
            var result = _controller.GetExactItem(1, 1) as OkNegotiatedContentResult<order_items>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.item_id);
        }

        [TestMethod]
        public void Putorder_items_UpdatesExistingItem()
        {
            // Arrange
            var mockOrderItems = new List<order_items>
            {
                new order_items { order_id = 1, item_id = 1, product_id=20, quantity = 5, list_price = 500, discount = 0.5m }
            }.AsQueryable();

            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.Provider).Returns(mockOrderItems.Provider);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.Expression).Returns(mockOrderItems.Expression);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.ElementType).Returns(mockOrderItems.ElementType);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.GetEnumerator()).Returns(mockOrderItems.GetEnumerator());

            _mockDbContext.Setup(db => db.order_items).Returns(_mockOrderItemsSet.Object);

            var updatedItem = new order_items { order_id = 1, item_id = 1, product_id = 20, quantity = 5, list_price = 100, discount = 0.2m };

            // Act
            var result = _controller.Putorder_items(1, 1, updatedItem) as StatusCodeResult;

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        //[TestMethod]
        //public void Postorder_items_AddsNewOrderItem()
        //{
        //    // Arrange
        //    var newItem = new order_items { order_id = 2, item_id = 3, quantity = 5, list_price = 25, discount = 0.2m };

        //    // Act
        //    var result = _controller.Postorder_items(newItem) as OkNegotiatedContentResult<order_items>;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(newItem.item_id, result.Content.item_id);
        //}

        [TestMethod]
        public void Deleteorder_items_RemovesOrderItem()
        {
            // Arrange
            var mockOrderItems = new List<order_items>
            {
                new order_items { order_id = 1, item_id = 1, product_id=20, quantity = 5, list_price = 500, discount = 0.5m }
            }.AsQueryable();

            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.Provider).Returns(mockOrderItems.Provider);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.Expression).Returns(mockOrderItems.Expression);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.ElementType).Returns(mockOrderItems.ElementType);
            _mockOrderItemsSet.As<IQueryable<order_items>>().Setup(m => m.GetEnumerator()).Returns(mockOrderItems.GetEnumerator());

            _mockDbContext.Setup(db => db.order_items).Returns(_mockOrderItemsSet.Object);

            // Act
            var result = _controller.Deleteorder_items(1, 1) as OkNegotiatedContentResult<order_items>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.item_id);
        }
    }
}
