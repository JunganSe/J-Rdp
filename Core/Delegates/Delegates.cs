using Core.Models;

namespace Core.Delegates;

// TODO: Use a ProfileInfo class instead? Would hold id, name, enabled status.
public delegate void ProfileHandler(List<ProfileInfo> profileInfos);
