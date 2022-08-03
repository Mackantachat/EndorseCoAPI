using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EndorseCoAPI.DataAccress;
using EndorseCoAPI.DataContract;


namespace EndorseCoAPI.BusinessLogic
{
    public class ServiceAction
    {
        private readonly string connectionString;
        public ServiceAction(string ConnectionName) => this.connectionString = ConnectionName;

        public void AddP_POL_EXCLUSION(ref P_POL_EXCLUSION addObject) => AddP_POL_EXCLUSION(ref  addObject,  null);
        private void AddP_POL_EXCLUSION(ref P_POL_EXCLUSION addObject , Repository repository)
        {
            bool internalConnection = false;
            if (repository == null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                repository.beginTransaction();
                internalConnection = true;
            }

            try
            {
                repository.AddP_POL_EXCLUSION(ref addObject);
                if (internalConnection)
                {
                    repository.commitTransaction();
                }
            }
            catch (Exception ex)
            {
                if (internalConnection)
                {
                    repository.rollbackTransaction();
                }
                throw ex;
            }
            finally
            {
                if (internalConnection)
                {
                    repository.CloseConnection();
                }
            }
        }

        public void GetP_POL_EXCLUSION(out P_POL_EXCLUSION[] dataObjects, long[] EXCLUDE_IDs, long[] POLICY_IDs) => GetP_POL_EXCLUSION(out dataObjects, EXCLUDE_IDs, POLICY_IDs , null);
        private void GetP_POL_EXCLUSION(out P_POL_EXCLUSION[] dataObjects, long[] EXCLUDE_IDs, long[] POLICY_IDs, Repository repository)
        {
            bool internalConnection = false;
            if (repository == null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                repository.beginTransaction();
                internalConnection = true;
            }

            try
            {
                repository.GetP_POL_EXCLUSION(out dataObjects,EXCLUDE_IDs, POLICY_IDs);
                if (internalConnection)
                {
                    repository.commitTransaction();
                }
            }
            catch (Exception ex)
            {
                if (internalConnection)
                {
                    repository.rollbackTransaction();
                }
                throw ex;
            }
            finally
            {
                if (internalConnection)
                {
                    repository.CloseConnection();
                }
            }
        }

        public void EditP_POL_EXCLUSION(ref P_POL_EXCLUSION updateObject)
        {
            EditP_POL_EXCLUSION(ref updateObject, null);
        }
        public void EditP_POL_EXCLUSION(ref P_POL_EXCLUSION updateObject, Repository repository)
        {
            bool internalConnection = false;
            if (repository == null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                repository.beginTransaction();
                internalConnection = true;
            }
            try
            {
                repository.EditP_POL_EXCLUSION(ref updateObject);
                if (updateObject.POL_EXCLUSION_DETAIL_Collection != null)
                {
                    for (int i = 0; i < updateObject.POL_EXCLUSION_DETAIL_Collection.Count(); i = i + 1)
                    {
                        if (updateObject.POL_EXCLUSION_DETAIL_Collection[i] != null)
                        {
                            updateObject.POL_EXCLUSION_DETAIL_Collection[i].EXCLUDE_ID = updateObject.EXCLUDE_ID;
                            P_POL_EXCLUSION_DETAIL detailObj = updateObject.POL_EXCLUSION_DETAIL_Collection[i];
                            EditP_POL_EXCLUSION_DETAIL(ref detailObj, repository);
                            updateObject.POL_EXCLUSION_DETAIL_Collection[i] = detailObj;
                        }
                    }
                }
                if (updateObject.POL_EXCLUSION_TMN != null)
                {
                    updateObject.POL_EXCLUSION_TMN.EXCLUDE_ID = updateObject.EXCLUDE_ID;
                    P_POL_EXCLUSION_TMN detailObj = updateObject.POL_EXCLUSION_TMN;
                    EditP_POL_EXCLUSION_TMN(ref detailObj, repository);
                    updateObject.POL_EXCLUSION_TMN = detailObj;
                }
                if (internalConnection)
                {
                    repository.commitTransaction();
                    repository.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                if (internalConnection)
                {
                    repository.rollbackTransaction();
                    repository.CloseConnection();
                }
                throw ex;
            }
        }

        public void EditP_POL_EXCLUSION_TMN(ref P_POL_EXCLUSION_TMN updateObject)
        {
            EditP_POL_EXCLUSION_TMN(ref updateObject, null);
        }
        public void EditP_POL_EXCLUSION_TMN(ref P_POL_EXCLUSION_TMN updateObject, Repository repository)
        {
            bool internalConnection = false;
            if (repository == null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                repository.beginTransaction();
                internalConnection = true;
            }
            try
            {
                repository.EditP_POL_EXCLUSION_TMN(ref updateObject);
                if (internalConnection)
                {
                    repository.commitTransaction();
                    repository.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                if (internalConnection)
                {
                    repository.rollbackTransaction();
                    repository.CloseConnection();
                }
                throw ex;
            }
        }

        public void EditP_POL_EXCLUSION_DETAIL(ref P_POL_EXCLUSION_DETAIL updateObject)
        {
            EditP_POL_EXCLUSION_DETAIL(ref updateObject, null);
        }
        public void EditP_POL_EXCLUSION_DETAIL(ref P_POL_EXCLUSION_DETAIL updateObject, Repository repository)
        {
            bool internalConnection = false;
            if (repository == null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                repository.beginTransaction();
                internalConnection = true;
            }
            try
            {
                repository.EditP_POL_EXCLUSION_DETAIL(ref updateObject);
                if (updateObject.POL_EXCLUSION_DETTMN != null)
                {
                    updateObject.POL_EXCLUSION_DETTMN.EXCLUDE_DET_ID = updateObject.EXCLUDE_DET_ID;
                    P_POL_EXCLUSION_DETTMN detailObj = updateObject.POL_EXCLUSION_DETTMN;
                    EditP_POL_EXCLUSION_DETTMN(ref detailObj, repository);
                    updateObject.POL_EXCLUSION_DETTMN = detailObj;
                }
                if (updateObject.POL_EXCLUSION_BYPLAN_Collection != null)
                {
                    for (int i = 0; i < updateObject.POL_EXCLUSION_BYPLAN_Collection.Count(); i = i + 1)
                    {
                        if (updateObject.POL_EXCLUSION_BYPLAN_Collection[i] != null)
                        {
                            updateObject.POL_EXCLUSION_BYPLAN_Collection[i].EXCLUDE_DET_ID = updateObject.EXCLUDE_DET_ID;
                            P_POL_EXCLUSION_BYPLAN detailObj = updateObject.POL_EXCLUSION_BYPLAN_Collection[i];
                            EditP_POL_EXCLUSION_BYPLAN(ref detailObj, repository);
                            updateObject.POL_EXCLUSION_BYPLAN_Collection[i] = detailObj;
                        }
                    }
                }
                if (internalConnection)
                {
                    repository.commitTransaction();
                    repository.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                if (internalConnection)
                {
                    repository.rollbackTransaction();
                    repository.CloseConnection();
                }
                throw ex;
            }
        }

        public void EditP_POL_EXCLUSION_DETTMN(ref P_POL_EXCLUSION_DETTMN updateObject)
        {
            EditP_POL_EXCLUSION_DETTMN(ref updateObject, null);
        }
        public void EditP_POL_EXCLUSION_DETTMN(ref P_POL_EXCLUSION_DETTMN updateObject, Repository repository)
        {
            bool internalConnection = false;
            if (repository == null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                repository.beginTransaction();
                internalConnection = true;
            }
            try
            {
                repository.EditP_POL_EXCLUSION_DETTMN(ref updateObject);
                if (internalConnection)
                {
                    repository.commitTransaction();
                    repository.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                if (internalConnection)
                {
                    repository.rollbackTransaction();
                    repository.CloseConnection();
                }
                throw ex;
            }
        }
        public void EditP_POL_EXCLUSION_BYPLAN(ref P_POL_EXCLUSION_BYPLAN updateObject)
        {
            EditP_POL_EXCLUSION_BYPLAN(ref updateObject, null);
        }
        public void EditP_POL_EXCLUSION_BYPLAN(ref P_POL_EXCLUSION_BYPLAN updateObject, Repository repository)
        {
            bool internalConnection = false;
            if (repository == null)
            {
                repository = new Repository(connectionString);
                repository.OpenConnection();
                repository.beginTransaction();
                internalConnection = true;
            }
            try
            {
                repository.EditP_POL_EXCLUSION_BYPLAN(ref updateObject);
                if (internalConnection)
                {
                    repository.commitTransaction();
                    repository.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                if (internalConnection)
                {
                    repository.rollbackTransaction();
                    repository.CloseConnection();
                }
                throw ex;
            }
        }
    }
}
