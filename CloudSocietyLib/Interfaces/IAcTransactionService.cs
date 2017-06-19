using System;

namespace CloudSocietyLib.Interfaces
{
    public interface IAcTransactionService : IAcTransactionRepository
    {
        String GetAcNatureByDocType(String doctype);
    }
}
