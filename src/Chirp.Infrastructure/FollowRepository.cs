using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure
{
    public class FollowRepository : IFollowRepository
    {
        private ChirpDBContext context;

        public FollowRepository(ChirpDBContext context)
        {
            this.context = context;
        }


        public async Task<List<string>> GetFollowerIDsByAuthorIDAsync(string AuthorID)
        {
            var followerIDs = await context.Followings
            .Where(f => f.FollowingId == AuthorID)
            .Select(f => f.FollowerId)
            .ToListAsync();
            return followerIDs;
        }

        public async Task<List<string>> GetFollowingIDsByAuthorIDAsync(string AuthorID)
        {
            var followingIDs = await context.Followings
            .Where(f => f.FollowerId == AuthorID)
            .Select(f => f.FollowingId)
            .ToListAsync();

            return followingIDs;
        }

        public async Task InsertNewFollowAsync(string FollowerID, string FollowingID)
        {
            context.Followings.Add(new Follow() { FollowerId = FollowerID, FollowingId = FollowingID, Timestamp = DateTime.Now });
            await context.SaveChangesAsync();
        }

        public async Task RemoveFollowAsync(string FollowerID, string FollowingID)
        {
            var follow = await context.Followings.FindAsync(FollowerID, FollowingID);
            if (follow != null)
            {
                context.Remove(follow);
                await context.SaveChangesAsync();
            }
        }
    }
}