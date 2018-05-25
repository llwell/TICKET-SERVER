using Com.ACBC.Framework.Database;
namespace Ticket_Server.Dao
{
    public class UserDao
    {

        public UserDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
    }
}
