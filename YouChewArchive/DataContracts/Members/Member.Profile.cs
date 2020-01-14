using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouChewArchive.Data;
using YouChewArchive.Logic;

namespace YouChewArchive.DataContracts
{
    public partial class Member
    {

        private ProfileFields profileFields;
        private List<Blog> blogs;
        private List<UserAward> awards;
        private List<string> previousNames;
        private List<string> previousStaffs;
        private List<WarnLog> warnLog;
        private MemberBannedInfo bannedInfo;

        [Ignore]
        public bool Contributed { get; set; } = false;

        [Ignore]
        public MemberBannedInfo BannedInfo
        {
            get
            {
                if(IsBanned() && bannedInfo == null)
                {
                    bannedInfo = DB.Instance.GetRecordById<MemberBannedInfo>(Id);
                }

                return bannedInfo;
            }
        }

        [Ignore]
        public List<WarnLog> WarnLogs
        {
            get
            {
                if(IsBanned() && warnLog == null)
                {
                    warnLog = DB.Instance.GetData<WarnLog>($"SELECT * FROM {WarnLog.TableName} WHERE wl_member = {Id} ORDER BY wl_date DESC");
                }

                return warnLog;
            }
        }

        [Ignore]
        public int? BannedDate
        {
            get
            {
                if(!IsBanned())
                {
                    return null;
                }

                if (IsPermBanned())
                {
                    return BannedInfo?.member_banned_date ?? last_activity ?? WarnLogs.Max(wl => wl.date);
                }

                return WarnLogs.Max(wl => wl.date);
                
            }
        }

        [Ignore]
        public ProfileFields ProfileFields
        {
            get
            {
                if(profileFields == null)
                {
                    profileFields = DB.Instance.GetRecordById<ProfileFields>(Id) ?? new ProfileFields() { member_id = Id };
                }

                return profileFields;
            }
        }

        [Ignore]
        public List<Blog> Blogs
        {
            get
            {
                if(blogs == null)
                {
                    blogs = BlogLogic.GetBlogs(this);
                }

                return blogs;
            }
        }

        public bool HasBlogs()
        {
            return Blogs.Count > 0;
        }

        [Ignore]
        public List<UserAward> Awards
        {
            get
            {
                if(awards == null)
                {
                    awards = DB.Instance.GetData<UserAward>($"SELECT * FROM {UserAward.TableName} WHERE user_id = {Id} ORDER BY date");
                }

                return awards;
            }
        }

        public bool HasAwards()
        {
            return Awards.Count > 0;
        }

        [Ignore]
        public List<string> PreviousNames
        {
            get
            {
                if(previousNames == null)
                {
                    previousNames = new List<string>();

                    List<MemberHistory> history = DB.Instance.GetData<MemberHistory>($"SELECT * FROM {MemberHistory.TableName} WHERE log_app = 'core' AND log_member = {Id} AND log_type='display_name' ORDER BY log_date");

                    int previousDate = joined;

                    foreach(MemberHistory mh in history)
                    {
                        if (mh.date - previousDate > (86400 * 7))
                        {

                            dynamic data = JObject.Parse(mh.data);

                            string oldName = data.old;

                            if (oldName != name && !String.IsNullOrWhiteSpace(oldName) && !previousNames.Contains(oldName))
                            {
                                previousNames.Insert(0, oldName);
                            }
                        }

                        previousDate = (int)mh.date;

                    }
                }

                return previousNames;
            }
        }

        public bool HasPreviousNames()
        {
            return PreviousNames.Count > 0;
        }

        public bool HasPreviousStaffs()
        {
            return PreviousStaffs.Count > 0;
        }

        public bool WasOwner()
        {
            switch(Id)
            {
                case 11018: //Conrad
                case 39: //RabbitSnore
                case 44: //TINS
                case 41: //Dopply
                case 120: //SuperYoshi
                case 160: //tabull
                case 246: //Whelt
                    return true;
                default:
                    return false;

            }
        }

        public bool WasAdmin()
        {
            if(WasOwner() || IsInGroup(4))
            {
                return true;
            }

            switch(Id)
            {
                case 90: //NegroTed
                case 198: //Yaminomalex
                case 131: //Emperor Ing
                case 15761: //Dark Fox
                case 16329: //ravinrabbid
                    return true;
                default:
                    return false;
            }
        }

        public bool WasModerator()
        {
            if(IsInGroup(6) || Id == 65)
            {
                return true;
            }

            if(WasAdmin())
            {
                switch(Id)
                {
                    case 160: //tabull
                    case 11018: //conrad
                    case 39: //RabbitSnore
                    case 44: //TINS
                    case 41: //Dopply
                        return false;
                    default:
                        return true;
                }
            }

            if (Awards.Any(a => a.award_id == 6) || IsInGroup(6))
            {
                return true;
            }

            return false;
        }

        public bool WasStaff()
        {
            return WasOwner() || WasAdmin() || WasModerator();
        }

        public bool WasReviewCrew()
        {
            switch(Id)
            {
                case 14355: //Aesaun
                case 1060: //Saiko
                case 621: //thewafflemaster
                case 53: //Eutinbon
                case 71: //RealGenericFilms
                case 97: //TheDarkRises
                case 379: //Mynn
                    return true;
                default:
                    return false;
            }
        }

        public bool WasWritingStaff()
        {
            //UncleChuck
            if(WasEditor())
            {
                return true;
            }

            if (WasStaff() || WasStaffArtist())
            {
                return false;
            }

            bool hasAward = Awards.Any(a => a.award_id == 3);

            return hasAward && ForumLogic.WasParticipant(this, 23);
        }

        public bool WasTechStaff()
        {
            if(Id == 160 || Id == 50) //tabull, Steg
            {
                return true;
            }

            if(WasStaff())
            {
                return false;
            }

            return ForumLogic.WasParticipant(this, 54);
        }

        public bool WasFreelancer()
        {
            return Id == 87 || Id == 175;
        }

        public bool WasStaffArtist()
        {
            if(WasStaff() || WasFreelancer() || Awards.Any(a => a.award_id == 3))
            {
                return false;
            }

            switch(Id)
            {
                case 58: //Black Puffin
                case 74: //Coretes
                case 100: //Ishkibibl
                case 175: //PUllahoko
                case 56: //Wikiwow
                case 110: //Yenehckcid
                case 317: //Kingplaypus
                case 1576: //Dippy
                case 137: //seanvol
                case 378: //crazythesecond
                    return true;
            }

            return ForumLogic.WasParticipant(this, 55, null, 1283385600);  //Before Sept 2, 2018

            
        }

        public bool WasEditorInChief()
        {
            switch (Id)
            {
                case 39:    //Rabbitsnore
                case 239:   //Ghoulston
                case 10878: //LightningLuigi
                case 1689:  //Spiral
                case 719:   //Nozdordomu
                case 11346: //Crazy Luigi
                    return true;
                default:
                    return false;
            }
        }

        public bool WasEditor()
        {
            switch(Id)
            {
                case 239:   //Ghoulston
                case 10878: //LightningLuigi
                case 1689:  //Spiral
                case 719:   //Nozdordomu
                case 11346: //Crazy Luigi
                case 15729: //TheOneManBoxOffice
                case 15192: //HerrVarden
                case 2823:  //supreme_slayer
                case 414:   //Liebermintz
                case 108:   //sonicnerd23
                case 94:    //Captpan6
                case 2101:  //spennyrinko
                case 123:   //Bematt
                case 45:    //UncleChuckTH
                case 109:   //SkyBlueFox
                case 711:   //Ride
                case 13089: //Vorhias
                case 12479: //Full Metal Kiwi
                    return true;
                default:
                    return false;
            }
        }

        public bool WasDesignStaff()
        {
            if(WasStaff() || WasTechStaff() || Id == 409)
            {
                return false;
            }

            if(ForumLogic.WasParticipant(this, 131))
            {
                return true;
            }

            if(WasWritingStaff() || WasReviewCrew())
            {
                return false;
            }

            //In Original Design Staff forum after September 4
            return ForumLogic.WasParticipant(this, 55, 1283558400, null);
        }

        public bool IsBanned()
        {
            return temp_ban.GetValueOrDefault(0) != 0;
        }

        public bool IsTempBanned()
        {
            return temp_ban > 0;
        }

        public bool IsPermBanned()
        {
            return temp_ban == -1;
        }

        public bool IsInGroup(int groupId)
        {
            return member_group_id == groupId || mgroup_others.Split(" ,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Any(g => g == groupId.ToString());
        }

        public bool ShowPreviousStaffs()
        {
            return !IsBanned() && Id != 14501 && Id != 12500 && Id != 42;
        }

        [Ignore]
        public List<string> PreviousStaffs
        {
            get
            {
                if(previousStaffs == null)
                {
                    previousStaffs = new List<string>();

                    if(!ShowPreviousStaffs())
                    {
                        return previousStaffs;
                    }

                    if(WasOwner())
                    {
                        previousStaffs.Add("<span style='color:#1b7dbf'>Owner</span>");
                    }

                    if(WasAdmin())
                    {
                       previousStaffs.Add(MemberLogic.GetGroupName(4));
                    }

                    if(WasModerator())
                    {
                        previousStaffs.Add(MemberLogic.GetGroupName(6));
                    }

                    if(WasEditorInChief())
                    {
                        previousStaffs.Add("<span style='color:turquoise'>Editor-in-Cheif'</span>");
                    }

                    if(WasEditor())
                    {
                        previousStaffs.Add("<span style='color:turquoise'>Writing Staff Editor</span>");
                    }

                    if (IsInGroup(9) || WasWritingStaff())
                    {
                        previousStaffs.Add(MemberLogic.GetGroupName(9));
                    }

                    //Cafe Staff
                    if (Awards.Any(a => a.award_id == 79))
                    {
                        previousStaffs.Add(MemberLogic.GetGroupName(11));
                    }

                    // Janitor
                    if(Awards.Any(a => a.award_id == 67))
                    {
                        previousStaffs.Add("<span style='color:red'>Janitor</span>");
                    }

                    

                    if(WasTechStaff())
                    {
                        previousStaffs.Add("<span style='color:orange'>Technical Staff</span>");
                    }

                    if(IsInGroup(60) || WasDesignStaff())
                    {
                        previousStaffs.Add(MemberLogic.GetGroupName(60));
                    }

                    if(WasStaffArtist())
                    {
                        previousStaffs.Add("<span style='color:orangered'>Staff Artist</span>");
                    }

                    if(WasFreelancer())
                    {
                        previousStaffs.Add("<span style='color:purple'>Freelancer</span>");
                    }

                    if(WasReviewCrew())
                    {
                        previousStaffs.Add("<span style='color:teal'>Review Crew</span>");
                    }
                    
                }

                return previousStaffs;
            }
        }

       
    }
}
