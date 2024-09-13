using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebAPI.Controllers;
using WebAPI.Models;

namespace WebUnitTest
{
    [TestClass]
    public class BrandTest
    {

        private Mock<BikeStores_Team3Entities> _mockDbContext;
        private Mock<DbSet<brand>> _mockBrandSet;
        private brandController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the mock context and controller
            _mockDbContext = new Mock<BikeStores_Team3Entities>();
            _mockBrandSet = new Mock<DbSet<brand>>();

            _controller = new brandController
            {
                db = _mockDbContext.Object
            };
        }
        [TestMethod]
        public void Getorder_items_ReturnsBrandById()
        {
            // Arrange
            var mockBrand = new List<brand>
            {
                new brand { brand_id = 1, brand_name = "Electra" },
                new brand { }
            }.AsQueryable();

            _mockBrandSet.As<IQueryable<brand>>().Setup(m => m.Provider).Returns(mockBrand.Provider);
            _mockBrandSet.As<IQueryable<brand>>().Setup(m => m.Expression).Returns(mockBrand.Expression);
            _mockBrandSet.As<IQueryable<brand>>().Setup(m => m.ElementType).Returns(mockBrand.ElementType);
            _mockBrandSet.As<IQueryable<brand>>().Setup(m => m.GetEnumerator()).Returns(mockBrand.GetEnumerator());

            _mockDbContext.Setup(db => db.brands).Returns(_mockBrandSet.Object);

            // Act
            var result = _controller.Getbrands() as IEnumerable<brand>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
    }
}
