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
    public class CategoryTest
    {
        private Mock<BikeStores_Team3Entities> _mockDbContext;
        private Mock<DbSet<category>> _mockCategorySet;
        private categoryController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the mock context and controller
            _mockDbContext = new Mock<BikeStores_Team3Entities>();
            _mockCategorySet = new Mock<DbSet<category>>();

            _controller = new categoryController
            {
                db = _mockDbContext.Object
            };
        }
        [TestMethod]
        public void GetCategory_ReturnsCategoryById()
        {
            // Arrange
            var mockCategory = new List<category>
            {
                new category { category_id = 1, category_name = "Children Bicycles" },
                new category { }
            }.AsQueryable();

            _mockCategorySet.As<IQueryable<category>>().Setup(m => m.Provider).Returns(mockCategory.Provider);
            _mockCategorySet.As<IQueryable<category>>().Setup(m => m.Expression).Returns(mockCategory.Expression);
            _mockCategorySet.As<IQueryable<category>>().Setup(m => m.ElementType).Returns(mockCategory.ElementType);
            _mockCategorySet.As<IQueryable<category>>().Setup(m => m.GetEnumerator()).Returns(mockCategory.GetEnumerator());

            _mockDbContext.Setup(db => db.categories).Returns(_mockCategorySet.Object);

            // Act
            var result = _controller.Getcategories() as IEnumerable<category>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
    }
}
