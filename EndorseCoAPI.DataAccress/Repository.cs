using DataAccessUtility;
using Oracle.ManagedDataAccess.Client;
using System;
using EndorseCoAPI.DataContract;
using ITUtility;
using System.Data;
using System.Linq;

namespace EndorseCoAPI.DataAccress
{
    public class Repository : RepositoryBaseManagedCore
    {
        public Repository(string ConnectionName) : base(ConnectionName)
        {

        }
        public Repository(OracleConnection connection) : base(connection)
        {

        }

        public void AddP_POL_EXCLUSION(ref P_POL_EXCLUSION addObject)
        {
            string sqlStr;
            OracleCommand oCmd;
            sqlStr = "SELECT P_POL_EXCLUSION_SEQ.NEXTVAL FROM DUAL";
            oCmd = new OracleCommand(sqlStr, connection);
            addObject.EXCLUDE_ID = Convert.ToInt64(oCmd.ExecuteScalar());

            sqlStr =  @"INSERT INTO POLICY.P_POL_EXCLUSION(
                   EXCLUDE_ID,
                   POLICY_ID,
                   EXCLUDE_TRN_DT,
                   EFF_DT,
                   REF_DT,
                   REFERENCE,
                   UNDERWRITE_ID,
                   TMN,
                   UPD_DT,
                   UPD_ID,
                   MINI_UPDATE,
                   TMN_EFF_DT,
                   MINI_DT)
                VALUES(
                   :EXCLUDE_ID,
                   :POLICY_ID,
                   :EXCLUDE_TRN_DT,
                   :EFF_DT,
                   :REF_DT,
                   :REFERENCE,
                   :UNDERWRITE_ID,
                   :TMN,
                   :UPD_DT,
                   :UPD_ID,
                   :MINI_UPDATE,
                   :TMN_EFF_DT,
                   :MINI_DT)" ;
            oCmd = new OracleCommand(sqlStr, connection);
            oCmd.Parameters.Add(new OracleParameter("EXCLUDE_ID", addObject.EXCLUDE_ID));
            oCmd.Parameters.Add(new OracleParameter("POLICY_ID", addObject.POLICY_ID));
            oCmd.Parameters.Add(new OracleParameter("EXCLUDE_TRN_DT", addObject.EXCLUDE_TRN_DT));
            oCmd.Parameters.Add(new OracleParameter("EFF_DT", addObject.EFF_DT));
            oCmd.Parameters.Add(new OracleParameter("REF_DT", addObject.REF_DT));
            oCmd.Parameters.Add(new OracleParameter("REFERENCE", addObject.REFERENCE));
            oCmd.Parameters.Add(new OracleParameter("UNDERWRITE_ID", addObject.UNDERWRITE_ID));
            oCmd.Parameters.Add(new OracleParameter("TMN", addObject.TMN));
            oCmd.Parameters.Add(new OracleParameter("UPD_DT", addObject.UPD_DT));
            oCmd.Parameters.Add(new OracleParameter("UPD_ID", addObject.UPD_ID));
            oCmd.Parameters.Add(new OracleParameter("MINI_UPDATE", addObject.MINI_UPDATE));
            oCmd.Parameters.Add(new OracleParameter("TMN_EFF_DT", addObject.TMN_EFF_DT));
            oCmd.Parameters.Add(new OracleParameter("MINI_DT", addObject.MINI_DT));
            int recordCount = oCmd.ExecuteNonQuery();
        }

        public void GetP_POL_EXCLUSION(out P_POL_EXCLUSION[] dataObjects, long[] EXCLUDE_IDs, long[] POLICY_IDs)
        {

            string condition = "";

            if (EXCLUDE_IDs != null)
            {
                string sIds = "";
                foreach (long id in EXCLUDE_IDs)
                {
                    if (sIds != "")
                        sIds = sIds + ", ";
                    sIds = sIds + Utility.SQLValueString(id);
                }

                if (condition == "")
                    condition = condition + "WHERE\n    ";
                else
                    condition = condition + "   AND ";
                condition = condition + "EXCLUDE_ID IN (" + sIds + ")\n";
            }

            if (POLICY_IDs != null)
            {
                string sIds = "";
                foreach (long id in POLICY_IDs)
                {
                    if (sIds != "")
                        sIds = sIds + ", ";
                    sIds = sIds + Utility.SQLValueString(id);
                }

                if (condition == "")
                    condition = condition + "WHERE\n    ";
                else
                    condition = condition + "   AND ";
                condition = condition + "POLICY_ID IN (" + sIds + ")\n";
            }

            string sqlStr =
                "SELECT * FROM \"POLICY\".P_POL_EXCLUSION \n" +
                condition;
            DataTable dt = Utility.FillDataTable(sqlStr, connection);
            dataObjects = dt.AsEnumerable<P_POL_EXCLUSION>().ToArray();
        }

        public  void EditP_POL_EXCLUSION(ref P_POL_EXCLUSION updateObject)
        {
            if (updateObject.EXCLUDE_ID == null)
            {
                AddP_POL_EXCLUSION(ref updateObject);
            }
            else
            {
                string sqlStr =
                    "UPDATE \"POLICY\".P_POL_EXCLUSION\n" +
                    "SET\n" +
                    "   POLICY_ID = " + Utility.SQLValueString(updateObject.POLICY_ID) + ",\n" +
                    "   EXCLUDE_TRN_DT = " + Utility.SQLValueString(updateObject.EXCLUDE_TRN_DT) + ",\n" +
                    "   EFF_DT = " + Utility.SQLValueString(updateObject.EFF_DT) + ",\n" +
                    "   REF_DT = " + Utility.SQLValueString(updateObject.REF_DT) + ",\n" +
                    "   REFERENCE = " + Utility.SQLValueString(updateObject.REFERENCE) + ",\n" +
                    "   UNDERWRITE_ID = " + Utility.SQLValueString(updateObject.UNDERWRITE_ID) + ",\n" +
                    "   TMN = " + Utility.SQLValueString(updateObject.TMN) + ",\n" +
                    "   UPD_DT = " + Utility.SQLValueString(updateObject.UPD_DT) + ",\n" +
                    "   UPD_ID = " + Utility.SQLValueString(updateObject.UPD_ID) + ",\n" +
                    "   TMN_EFF_DT = " + Utility.SQLValueString(updateObject.TMN_EFF_DT) + ",\n" +
                    "   MINI_UPDATE = " + Utility.SQLValueString(updateObject.MINI_UPDATE) + ",\n" +
                    "   MINI_DT = " + Utility.SQLValueString(updateObject.MINI_DT) + "\n" +
                    "WHERE\n" +
                     "   EXCLUDE_ID = " + Utility.SQLValueString(updateObject.EXCLUDE_ID) + "\n";
                OracleCommand oCmd = new OracleCommand(sqlStr, connection);
                int c = oCmd.ExecuteNonQuery();
            }
        }
        public  void EditP_POL_EXCLUSION_DETTMN(ref P_POL_EXCLUSION_DETTMN updateObject)
        {
            if (updateObject.EXCLUDE_DET_ID == null)
            {
                AddP_POL_EXCLUSION_DETTMN(ref updateObject);
            }
            else
            {
                string sqlStr =
                    "UPDATE \"POLICY\".P_POL_EXCLUSION_DETTMN\n" +
                    "SET\n" +
                    "   TMN_DT = " + Utility.SQLValueString(updateObject.TMN_DT) + ",\n" +
                    "   TMN_ID = " + Utility.SQLValueString(updateObject.TMN_ID) + ",\n" +
                    "   TRANSACTION_ID = " + Utility.SQLValueString(updateObject.TRANSACTION_ID) + "\n" +
                    "   EXCDET_TMN_CAUSE = " + Utility.SQLValueString(updateObject.EXCDET_TMN_CAUSE) + "\n" +
                    "WHERE\n" +
                     "   EXCLUDE_DET_ID = " + Utility.SQLValueString(updateObject.EXCLUDE_DET_ID) + "\n";
                OracleCommand oCmd = new OracleCommand(sqlStr, connection);
                int c = oCmd.ExecuteNonQuery();
                if (c == 0)
                {
                    AddP_POL_EXCLUSION_DETTMN(ref updateObject);
                }
            }
        }
        public  void AddP_POL_EXCLUSION_DETTMN(ref P_POL_EXCLUSION_DETTMN addObject)
        {
            string sqlStr;
            OracleCommand oCmd;


            sqlStr =
                "INSERT INTO \"POLICY\".P_POL_EXCLUSION_DETTMN(\n" +
                "   EXCLUDE_DET_ID,\n" +
                "   TMN_DT,\n" +
                "   TMN_ID,\n" +
                "   TRANSACTION_ID,\n" +
                "   EXCDET_TMN_CAUSE)\n" +
                "VALUES(\n" +
                "   " + Utility.SQLValueString(addObject.EXCLUDE_DET_ID) + ",\n" +
                "   " + Utility.SQLValueString(addObject.TMN_DT) + ",\n" +
                "   " + Utility.SQLValueString(addObject.TMN_ID) + ",\n" +
                "   " + Utility.SQLValueString(addObject.TRANSACTION_ID) + ",\n" +
                "   " + Utility.SQLValueString(addObject.EXCDET_TMN_CAUSE) + ")\n" +
                "";
            oCmd = new OracleCommand(sqlStr, connection);
            int recordCount = oCmd.ExecuteNonQuery();
        }
        public  void EditP_POL_EXCLUSION_DETAIL(ref P_POL_EXCLUSION_DETAIL updateObject)
        {
            if (updateObject.EXCLUDE_DET_ID == null)
            {
                AddP_POL_EXCLUSION_DETAIL(ref updateObject);
            }
            else
            {
                string sqlStr =
                    "UPDATE \"POLICY\".P_POL_EXCLUSION_DETAIL\n" +
                    "SET\n" +
                    "   EXCLUDE_ID = " + Utility.SQLValueString(updateObject.EXCLUDE_ID) + ",\n" +
                    "   EXCLUDE = " + Utility.SQLValueString(updateObject.EXCLUDE) + ",\n" +
                    "   ADMIT_DT = " + Utility.SQLValueString(updateObject.ADMIT_DT) + ",\n" +
                    "   EXCLUDE_CAUSE = " + Utility.SQLValueString(updateObject.EXCLUDE_CAUSE) + ",\n" +
                    "   ENDORSE_PRINTING = " + Utility.SQLValueString(updateObject.ENDORSE_PRINTING) + ",\n" +
                    "   TMN = " + Utility.SQLValueString(updateObject.TMN) + ",\n" +
                    "   MINI_UPDATE = " + Utility.SQLValueString(updateObject.MINI_UPDATE) + ",\n" +
                    "   MINI_DT = " + Utility.SQLValueString(updateObject.MINI_DT) + "\n" +
                    "WHERE\n" +
                     "   EXCLUDE_DET_ID = " + Utility.SQLValueString(updateObject.EXCLUDE_DET_ID) + "\n";
                OracleCommand oCmd = new OracleCommand(sqlStr, connection);
                int c = oCmd.ExecuteNonQuery();
                if (c == 0)
                {
                    AddP_POL_EXCLUSION_DETAIL(ref updateObject);
                }
            }
        }

        public  void AddP_POL_EXCLUSION_DETAIL(ref P_POL_EXCLUSION_DETAIL addObject)
        {
            string sqlStr;
            OracleCommand oCmd;

            sqlStr = "SELECT P_POL_EXCLUSION_DETAIL_SEQ.NEXTVAL FROM DUAL";
            oCmd = new OracleCommand(sqlStr, connection);
            addObject.EXCLUDE_DET_ID = Convert.ToInt64(oCmd.ExecuteScalar());

            sqlStr =
                "INSERT INTO \"POLICY\".P_POL_EXCLUSION_DETAIL(\n" +
                "   EXCLUDE_DET_ID,\n" +
                "   EXCLUDE_ID,\n" +
                "   EXCLUDE,\n" +
                "   ADMIT_DT,\n" +
                "   EXCLUDE_CAUSE,\n" +
                "   ENDORSE_PRINTING,\n" +
                "   TMN,\n" +
                "   MINI_UPDATE,\n" +
                "   MINI_DT)\n" +
                "VALUES(\n" +
                "   " + Utility.SQLValueString(addObject.EXCLUDE_DET_ID) + ",\n" +
                "   " + Utility.SQLValueString(addObject.EXCLUDE_ID) + ",\n" +
                "   " + Utility.SQLValueString(addObject.EXCLUDE) + ",\n" +
                "   " + Utility.SQLValueString(addObject.ADMIT_DT) + ",\n" +
                "   " + Utility.SQLValueString(addObject.EXCLUDE_CAUSE) + ",\n" +
                "   " + Utility.SQLValueString(addObject.ENDORSE_PRINTING) + ",\n" +
                "   " + Utility.SQLValueString(addObject.TMN) + ",\n" +
                "   " + Utility.SQLValueString(addObject.MINI_UPDATE) + ",\n" +
                "   " + Utility.SQLValueString(addObject.MINI_DT) + ")\n" +
                "";
            oCmd = new OracleCommand(sqlStr, connection);
            int recordCount = oCmd.ExecuteNonQuery();
        }
        public  void EditP_POL_EXCLUSION_BYPLAN(ref P_POL_EXCLUSION_BYPLAN updateObject)
        {
            if (updateObject.EXCLUDE_PLAN_ID == null)
            {
                AddP_POL_EXCLUSION_BYPLAN(ref updateObject);
            }
            else
            {
                string sqlStr =
                    "UPDATE \"POLICY\".P_POL_EXCLUSION_BYPLAN\n" +
                    "SET\n" +
                    "   EXCLUDE_DET_ID = " + Utility.SQLValueString(updateObject.EXCLUDE_DET_ID) + ",\n" +
                    "   PL_BLOCK = " + Utility.SQLValueString(updateObject.PL_BLOCK) + ",\n" +
                    "   PL_CODE = " + Utility.SQLValueString(updateObject.PL_CODE) + ",\n" +
                    "   PL_CODE2 = " + Utility.SQLValueString(updateObject.PL_CODE2) + "\n" +
                    "WHERE\n" +
                     "   EXCLUDE_PLAN_ID = " + Utility.SQLValueString(updateObject.EXCLUDE_PLAN_ID) + "\n";
                OracleCommand oCmd = new OracleCommand(sqlStr, connection);
                int c = oCmd.ExecuteNonQuery();
            }
        }

        public  void AddP_POL_EXCLUSION_BYPLAN(ref P_POL_EXCLUSION_BYPLAN addObject)
        {
            string sqlStr;
            OracleCommand oCmd;

            sqlStr = "SELECT P_POL_EXCLUSION_BYPLAN_SEQ.NEXTVAL FROM DUAL";
            oCmd = new OracleCommand(sqlStr, connection);
            addObject.EXCLUDE_PLAN_ID = Convert.ToInt64(oCmd.ExecuteScalar());

            sqlStr =
                "INSERT INTO \"POLICY\".P_POL_EXCLUSION_BYPLAN(\n" +
                "   EXCLUDE_PLAN_ID,\n" +
                "   EXCLUDE_DET_ID,\n" +
                "   PL_BLOCK,\n" +
                "   PL_CODE,\n" +
                "   PL_CODE2)\n" +
                "VALUES(\n" +
                "   " + Utility.SQLValueString(addObject.EXCLUDE_PLAN_ID) + ",\n" +
                "   " + Utility.SQLValueString(addObject.EXCLUDE_DET_ID) + ",\n" +
                "   " + Utility.SQLValueString(addObject.PL_BLOCK) + ",\n" +
                "   " + Utility.SQLValueString(addObject.PL_CODE) + ",\n" +
                "   " + Utility.SQLValueString(addObject.PL_CODE2) + ")\n" +
                "";
            oCmd = new OracleCommand(sqlStr, connection);
            int recordCount = oCmd.ExecuteNonQuery();
        }

        public  void EditP_POL_EXCLUSION_TMN(ref P_POL_EXCLUSION_TMN updateObject)
        {
            if (updateObject.EXCLUDE_ID == null)
            {
                AddP_POL_EXCLUSION_TMN(ref updateObject);
            }
            else
            {
                string sqlStr =
                    "UPDATE \"POLICY\".P_POL_EXCLUSION_TMN\n" +
                    "SET\n" +
                    "   TMN_DT = " + Utility.SQLValueString(updateObject.TMN_DT) + ",\n" +
                    "   TMN_ID = " + Utility.SQLValueString(updateObject.TMN_ID) + ",\n" +
                    "   TRANSACTION_ID = " + Utility.SQLValueString(updateObject.TRANSACTION_ID) + "\n" +
                    "   EXC_TMN_CAUSE = " + Utility.SQLValueString(updateObject.EXC_TMN_CAUSE) + "\n" +
                    "WHERE\n" +
                     "   EXCLUDE_ID = " + Utility.SQLValueString(updateObject.EXCLUDE_ID) + "\n";
                OracleCommand oCmd = new OracleCommand(sqlStr, connection);
                int c = oCmd.ExecuteNonQuery();
                if (c == 0)
                {
                    AddP_POL_EXCLUSION_TMN(ref updateObject);
                }
            }
        }
        public  void AddP_POL_EXCLUSION_TMN(ref P_POL_EXCLUSION_TMN addObject)
        {
            string sqlStr;
            OracleCommand oCmd;


            sqlStr =
                "INSERT INTO \"POLICY\".P_POL_EXCLUSION_TMN(\n" +
                "   EXCLUDE_ID,\n" +
                "   TMN_DT,\n" +
                "   TMN_ID,\n" +
                "   TRANSACTION_ID,\n" +
                "   EXC_TMN_CAUSE)\n" +
                "VALUES(\n" +
                "   " + Utility.SQLValueString(addObject.EXCLUDE_ID) + ",\n" +
                "   " + Utility.SQLValueString(addObject.TMN_DT) + ",\n" +
                "   " + Utility.SQLValueString(addObject.TMN_ID) + ",\n" +
                "   " + Utility.SQLValueString(addObject.TRANSACTION_ID) + ",\n" +
                "   " + Utility.SQLValueString(addObject.EXC_TMN_CAUSE) + ")\n" +
                "";
            oCmd = new OracleCommand(sqlStr, connection);
            int recordCount = oCmd.ExecuteNonQuery();
        }
    }
}
