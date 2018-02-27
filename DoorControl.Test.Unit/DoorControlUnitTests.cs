﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace DoorControl.Test.Unit
{
    [TestFixture]
    public class DoorControlUnitTests
    {
        private IDoor _door;
        private IUserValidation _userValidation;
        private IEntryNotification _entryNotification;
        private IAlarm _alarm;
        private DoorControl _uut;

        [SetUp]
        public void Setup()
        {
            _door = Substitute.For<IDoor>();
            _userValidation = Substitute.For<IUserValidation>();
            _entryNotification = Substitute.For<IEntryNotification>();
            _alarm = Substitute.For<IAlarm>();
            _uut = new DoorControl(_door, _userValidation, _entryNotification, _alarm);
        }

        [Test]
        public void RequestEntryTest_ValidId_DoorOpenCalled()
        {
            // Arrange
            _userValidation.ValidateEntryRequest("test").ReturnsForAnyArgs(true);

            // Act
            _uut.RequestEntry("test");

            // Assert
            _door.Received().Open();
        }

        [Test]
        public void RequestEntryTest_ValidId_NotifyEntryGrantedCalled()
        {
            // Arrange
            _userValidation.ValidateEntryRequest("test").ReturnsForAnyArgs(true);

            // Act
            _uut.RequestEntry("test");

            // Assert
            _entryNotification.Received().NotifyEntryGranted();
        }

        [Test]
        public void RequestEntryTest_InvalidId_DoorOpenNotCalled()
        {
            // Arrange
            _userValidation.ValidateEntryRequest("test").ReturnsForAnyArgs(false);

            // Act
            _uut.RequestEntry("test");

            // Assert
            _door.DidNotReceive().Open();
        }

        [Test]
        public void RequestEntryTest_ValidId_NotifyEntryDeniedCalled()
        {
            // Arrange
            _userValidation.ValidateEntryRequest("test").ReturnsForAnyArgs(false);

            // Act
            _uut.RequestEntry("test");

            // Assert
            _entryNotification.Received().NotifyEntryDenied();
        }

        [Test]
        public void DoorOpenedEventHandlerTest_DoorIsOpened_CloseDoorIsCalled()
        {

            // Arrange
            _userValidation.ValidateEntryRequest("test").ReturnsForAnyArgs(true);

            //Act
            _uut.RequestEntry("test");
            _door.DoorOpenedEvent += Raise.EventWith(new DoorOpenedEventArgs());

            //Assert
            _door.Received().Close();
            _alarm.DidNotReceive().SignalAlarm();
        }

        [Test]
        public void DoorOpenedEventHandlerTest_DoorIsClosed_CloseDoorIsCalled()
        {
            
            // Act
            _door.DoorOpenedEvent += Raise.EventWith(new DoorOpenedEventArgs());


            // Assert
            _door.Received().Close();
        }

        [Test]
        public void DoorOpenedEventHandlerTest_DoorIsClosed_AlarmIsRaised()
        {
            // Act
            _door.DoorOpenedEvent += Raise.EventWith(new DoorOpenedEventArgs());


            // Assert
            _alarm.Received().SignalAlarm();

        }

        [Test]
        public void HandleDoorClosedEvent_DoorClosed_returnsTrue()
        {
            //arrange

            //Act
            _door.DoorClosedEvent += Raise.EventWith(new DoorClosedEventArgs());

        }
    }
}
