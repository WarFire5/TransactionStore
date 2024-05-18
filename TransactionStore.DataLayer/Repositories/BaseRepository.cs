namespace TransactionStore.DataLayer.Repositories;

public class BaseRepository
{
    protected readonly TransactionStoreContext _ctx;
    public BaseRepository(TransactionStoreContext context)
    {
        _ctx = context;
    }
}