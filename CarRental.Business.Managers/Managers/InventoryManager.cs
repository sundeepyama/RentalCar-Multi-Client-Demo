using CarRental.Business.Contracts.Service_Contracts;
using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Business.Managers.Managers
{
    public class InventoryManager: ManagerBase, IInventoryService
    {
        public InventoryManager()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public InventoryManager(IDataRepositoryFactory dataRepositoryFactory)
        {
            _DataRepositoryFactory = dataRepositoryFactory;
        }

        [Import]
        IDataRepositoryFactory _DataRepositoryFactory;

        public Entities.Car GetCar(int carId)
        {
            throw new NotImplementedException();
        }

        public Entities.Car[] GetAllCars()
        {
            throw new NotImplementedException();
        }
    }
}
