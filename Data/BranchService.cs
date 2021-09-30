using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class BranchService
    {
        public Task<Branches[]> GetBranches()
        {
            IDbConnection db = new FbConnection(Startup.VectorConnection);
            var branches = db.Query<Branches>("SELECT b.NAME, b.BRANCHID, b.PARENTID FROM VEBRANCHES b WHERE b.REMOVED IS NULL ORDER BY b.NAME").ToList();

            return Task.FromResult(Enumerable.Range(0, branches.Count).Select(index => new Branches
            {
                Name = branches[index].Name,
                BranchStringID = branches[index].BRANCHID.ToString(),
                BRANCHID = branches[index].BRANCHID,
                PARENTID = branches[index].PARENTID
            }).ToArray());
        }
    }
}
