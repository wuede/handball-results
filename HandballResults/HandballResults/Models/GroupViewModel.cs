using HandballResults.Services;

namespace HandballResults.Models
{
    public class GroupViewModel
    {
        public Group Group { get; set; }
        public ServiceException Error { get; set; }

        public GroupViewModel(Group group, ServiceException error)
        {
            Group = group;
            Error = error;
        }
    }
}