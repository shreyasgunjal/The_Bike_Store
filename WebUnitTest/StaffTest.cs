using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web.Http.Results;
using Castle.Core.Resource;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;
using WebAPI.Models;
namespace WebUnitTest
{
    [TestClass]
    public class StaffTest
    {
        private Mock<BikeStores_Team3Entities> _mockDbContext;
        private Mock<DbSet<staff>> _mockStaffSet;
        private staffController _controller;
        private Mock<DbSet<order>> _mockOrderSet;
        private Mock<DbSet<customer>> _mockCustomerSet;
        private Mock<DbSet<store>> _mockStoreSet;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the mock context and controller
            _mockDbContext = new Mock<BikeStores_Team3Entities>();
            _mockStaffSet = new Mock<DbSet<staff>>();

            _controller = new staffController
            {
                db = _mockDbContext.Object
            };
        }


        [TestMethod]
        public void GetAllStaff()
        {
            var mockStaff = new List<staff>
            {
                new staff { staff_id = 1, first_name = "Fabiola", last_name = "Jackson", email = "string", phone = "string", active = 1, store_id = 1, manager_id = 1, password = null },
                new staff { staff_id = 2, first_name = "Mireya", last_name = "Copeland", email = "mireya.copeland@bikes.shop", phone = "(831) 555-5555", active = 1, store_id = 1, manager_id = 1, password = null }
            }.AsQueryable();

            _mockStaffSet.As<IQueryable<staff>>().Setup(m => m.Provider).Returns(mockStaff.Provider);
            _mockStaffSet.As<IQueryable<staff>>().Setup(m => m.Expression).Returns(mockStaff.Expression);
            _mockStaffSet.As<IQueryable<staff>>().Setup(m => m.ElementType).Returns(mockStaff.ElementType);
            _mockStaffSet.As<IQueryable<staff>>().Setup(m => m.GetEnumerator()).Returns(mockStaff.GetEnumerator());

            _mockDbContext.Setup(db => db.staffs).Returns(_mockStaffSet.Object);

            // Act
            var result = _controller.Getstaffs() as IEnumerable<staff>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }


        [TestMethod]
        public void GetManager()
        {
            var staff_id = 10;
            var manager_id = 1;
            // Arrange
            var mockStaff = new List<staff>
            {

                new staff
                {
            staff_id = staff_id,
            first_name = "Yash",
            last_name = "Darak",
            email = "123@gmail.com",
            phone = "123",
            active = 0,
            store_id = 1,
            manager_id = 1,
            password = "Yash@123" },
             new staff
             {
               staff_id = manager_id, first_name="Yash", last_name= "Jackson", email= "Deve@123.gmail.com", phone= "8765445567",    active=1, store_id=1,manager_id=1, password="NULL"}
            }.AsQueryable();

            _mockStaffSet.As<IQueryable<staff>>().Setup(m => m.Provider).Returns(mockStaff.Provider);
            _mockStaffSet.As<IQueryable<staff>>().Setup(m => m.Expression).Returns(mockStaff.Expression);
            _mockStaffSet.As<IQueryable<staff>>().Setup(m => m.ElementType).Returns(mockStaff.ElementType);
            _mockStaffSet.As<IQueryable<staff>>().Setup(m => m.GetEnumerator()).Returns(mockStaff.GetEnumerator());

            _mockStaffSet.Setup(m => m.Find(It.Is<int>(id => id == staff_id)))
                  .Returns(mockStaff.FirstOrDefault(s => s.staff_id == staff_id));

            _mockStaffSet.Setup(m => m.Find(It.Is<int>(id => id == manager_id)))
                         .Returns(mockStaff.FirstOrDefault(s => s.staff_id == manager_id));

            _mockDbContext.Setup(db => db.staffs).Returns(_mockStaffSet.Object);

            // Act
            var result = _controller.GetManagerByStaffId(staff_id) as OkNegotiatedContentResult<staff>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Content.staff_id);
        }

        [TestMethod]
        public void GetStaffById()
        {
            // Arrange
            var staffId = 1;
            var mockStaff = new List<staff>
        {
            new staff { staff_id = staffId, first_name = "Fabiola", last_name = "Jackson", email = "string", phone = "string", active = 1, store_id = 1, manager_id = 1, password = null }
        }.AsQueryable();

            var mockStaffSet = new Mock<DbSet<staff>>();
            mockStaffSet.As<IQueryable<staff>>().Setup(m => m.Provider).Returns(mockStaff.Provider);
            mockStaffSet.As<IQueryable<staff>>().Setup(m => m.Expression).Returns(mockStaff.Expression);
            mockStaffSet.As<IQueryable<staff>>().Setup(m => m.ElementType).Returns(mockStaff.ElementType);
            mockStaffSet.As<IQueryable<staff>>().Setup(m => m.GetEnumerator()).Returns(mockStaff.GetEnumerator());

            // Mock the Find method to return the staff entity with the given ID
            mockStaffSet.Setup(m => m.Find(staffId)).Returns(mockStaff.SingleOrDefault(s => s.staff_id == staffId));

            _mockDbContext.Setup(db => db.staffs).Returns(mockStaffSet.Object);

            // Act
            var result = _controller.GetStaffById(staffId) as OkNegotiatedContentResult<staff>;

            // Assert
            Assert.IsNotNull(result, "Expected result to be OkNegotiatedContentResult<staff> but it was null.");
            Assert.IsNotNull(result.Content, "Expected result.Content to be not null.");
            Assert.AreEqual(staffId, result.Content.staff_id, "Staff ID did not match the expected value.");
        }

        [TestMethod]
        public void DeleteStaff()
        {
            // Arrange
            var staffIdToDelete = 10;

            var mockStaff = new List<staff>
         {
        new staff { staff_id = staffIdToDelete, first_name = "Yash", last_name = "Darak", email = "123@gmail.com", phone = "123", active = 0, store_id = 1, manager_id = 1, password = "Yash@123" }
         }.AsQueryable();

            var mockStaffSet = new Mock<DbSet<staff>>();
            mockStaffSet.As<IQueryable<staff>>().Setup(m => m.Provider).Returns(mockStaff.Provider);
            mockStaffSet.As<IQueryable<staff>>().Setup(m => m.Expression).Returns(mockStaff.Expression);
            mockStaffSet.As<IQueryable<staff>>().Setup(m => m.ElementType).Returns(mockStaff.ElementType);
            mockStaffSet.As<IQueryable<staff>>().Setup(m => m.GetEnumerator()).Returns(mockStaff.GetEnumerator());

            mockStaffSet.Setup(m => m.Find(staffIdToDelete))
                        .Returns(mockStaff.FirstOrDefault(s => s.staff_id == staffIdToDelete));

            mockStaffSet.Setup(m => m.Remove(It.IsAny<staff>())).Verifiable();

            _mockDbContext.Setup(db => db.staffs).Returns(mockStaffSet.Object);
            _mockDbContext.Setup(db => db.SaveChanges()).Returns(1); // Simulate successful save

            // Act
            var result = _controller.Deletestaff(staffIdToDelete) as OkNegotiatedContentResult<staff>;

            // Assert
            Assert.IsNotNull(result, "Expected result to be OkNegotiatedContentResult<staff> but it was null.");
            Assert.IsNotNull(result.Content, "Expected result.Content to be non-null.");
            Assert.AreEqual(staffIdToDelete, result.Content.staff_id, "Expected the staff_id to be deleted.");
            mockStaffSet.Verify(m => m.Remove(It.IsAny<staff>()), Times.Once, "Expected Remove to be called once.");
            _mockDbContext.Verify(db => db.SaveChanges(), Times.Once, "Expected SaveChanges to be called once.");
        }
    }
}

