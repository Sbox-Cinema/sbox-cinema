using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;
public static class ClientHelper
{
    public static IClient FindById(int clientId)
    {
        IClient client = null;
        if (clientId > 0)
        {
            client = Entity.FindByIndex(clientId) as IClient;
        }
        return client;
    }
}
