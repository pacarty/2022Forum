using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class ForumService : IForumService
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForumService(DataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> CheckDb()
        {
            if (await IsDbEmpty())
            {
                await SeedDb();
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> IsDbEmpty()
        {
            if (await _context.Subforums.CountAsync() > 0
                || await _context.Topics.CountAsync() > 0
                || await _context.Posts.CountAsync() > 0
                || await _context.Comments.CountAsync() > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task SeedDb()
        {
            List<ApplicationUser> users = new List<ApplicationUser>()
            {
                new ApplicationUser() { UserName = "boss", AuthAccessLevel = 4 },
                new ApplicationUser() { UserName = "patrick", AuthAccessLevel = 3 },
                new ApplicationUser() { UserName = "dude", AuthAccessLevel = 3 },
                new ApplicationUser() { UserName = "man", AuthAccessLevel = 1 },
                new ApplicationUser() { UserName = "dave", AuthAccessLevel = 1 },
                new ApplicationUser() { UserName = "john", AuthAccessLevel = 1 }
            };

            foreach (var user in users)
            {
                await _userManager.CreateAsync(user, "pass");
            }

            List<Subforum> subforumList = new List<Subforum>()
            {
                new Subforum() { Name = "Computers" },
                new Subforum() { Name = "Technology" },
                new Subforum() { Name = "ISPs" },
                new Subforum() { Name = "Mobile" }
            };

            List<Topic> topicList = new List<Topic>()
            {
                new Topic() { Name = "Desktops", SubforumId = 1 },
                new Topic() { Name = "Monitors/GPUs", SubforumId = 1 },
                new Topic() { Name = "Laptops", SubforumId = 1 },
                new Topic() { Name = "Peripherals", SubforumId = 1 },
                new Topic() { Name = "Modems/Routers", SubforumId = 2 },
                new Topic() { Name = "Networking", SubforumId = 2 },
                new Topic() { Name = "Telephony", SubforumId = 2 },
                new Topic() { Name = "Servers/Hosting", SubforumId = 2 },
                new Topic() { Name = "Choosing an ISP", SubforumId = 3 },
                new Topic() { Name = "Broadband", SubforumId = 3 },
                new Topic() { Name = "NBN", SubforumId = 3 },
                new Topic() { Name = "Telstra Broadband", SubforumId = 3 },
                new Topic() { Name = "Mobile carriers", SubforumId = 4 },
                new Topic() { Name = "Wireless ISPs", SubforumId = 4 },
                new Topic() { Name = "iPhone", SubforumId = 4 },
                new Topic() { Name = "Android phones", SubforumId = 4 }
            };

            List<Post> postList = new List<Post>()
            {
                new Post() { Name = "Geforce RTX Discussion", TopicId = 1, UserId = users[3].Id },
                new Post() { Name = "Cold boot Issues with Aorus X570 Pro Wifi Mobo", TopicId = 1, UserId = users[2].Id },
                new Post() { Name = "Intel Microarchitecture discussion - On the Lake.. ", TopicId = 1, UserId = users[3].Id },
                new Post() { Name = "49 ultrawide VS 34 ultrawide", TopicId = 1, UserId = users[0].Id },
                new Post() { Name = "Strange Text Issue with New PC", TopicId = 1, UserId = users[1].Id },
                new Post() { Name = "Mini ITX build is the following all looking ok", TopicId = 1, UserId = users[5].Id },
                new Post() { Name = "Your First and Favourite system", TopicId = 1, UserId = users[1].Id },
                new Post() { Name = "Australian PC Shops Guide v3.0 - Discussion Thread", TopicId = 1, UserId = users[2].Id },
                new Post() { Name = "AMD GPU discussion", TopicId = 1, UserId = users[5].Id },
                new Post() { Name = "AMD Zen microarchitecture", TopicId = 1, UserId = users[4].Id },
                new Post() { Name = "Monitor Drivers, Install them or don't worry?", TopicId = 1, UserId = users[0].Id },
                new Post() { Name = "Newer GPUs with older Dell Vostro 460", TopicId = 1, UserId = users[5].Id },
                new Post() { Name = "Alienware AW3423DW 34″ QD-OLED Panel & with 175HZ ", TopicId = 1, UserId = users[5].Id },
                new Post() { Name = "Dell U3223QE or U3421WE Monitor", TopicId = 1, UserId = users[5].Id },
                new Post() { Name = "Replacement PC sanity check", TopicId = 1, UserId = users[4].Id },
                new Post() { Name = "Reset Dell Inspiron 5675 for selling", TopicId = 1, UserId = users[2].Id },
                new Post() { Name = "Pre-built/Custom PC builds", TopicId = 1, UserId = users[3].Id },
                new Post() { Name = "Repairs in Brisbane", TopicId = 1, UserId = users[3].Id },
                new Post() { Name = "Where to order Nvidia 3000 cards", TopicId = 1, UserId = users[1].Id },
                new Post() { Name = "Intel ARC Discussion ", TopicId = 1, UserId = users[1].Id },
                new Post() { Name = "New Motherboard - Higher Power and Lower Score", TopicId = 1, UserId = users[1].Id },
                new Post() { Name = "Gaming build ~$3k budget", TopicId = 1, UserId = users[5].Id },
                new Post() { Name = "Fair Pricing for a Used Gaming PC", TopicId = 1, UserId = users[3].Id },
                new Post() { Name = "Case / cooler advice", TopicId = 1, UserId = users[3].Id },
                new Post() { Name = "GPU rec — HDMI 2.1 for 4K 120Hz", TopicId = 1, UserId = users[1].Id },
                new Post() { Name = "Will this docking station allow triple monitors?", TopicId = 1, UserId = users[3].Id },
                new Post() { Name = "Best Low Profile GPU - GTX1650 Vs New AMD RX6400", TopicId = 1, UserId = users[0].Id },
                new Post() { Name = "suggest a video card / 5600X 4000D budget build", TopicId = 1, UserId = users[0].Id },
                new Post() { Name = "BIOS has long boot time", TopicId = 1, UserId = users[1].Id },
                new Post() { Name = "The Current State of SSDs", TopicId = 1, UserId = users[5].Id },
                new Post() { Name = "SFF PC or Rasberry Pi?", TopicId = 1, UserId = users[0].Id },
                new Post() { Name = "Budget Gaming Build - Feedback would be great", TopicId = 1, UserId = users[3].Id },
                new Post() { Name = "Strange startup behaviour", TopicId = 1, UserId = users[0].Id },
                new Post() { Name = "Help with Upgrade Advice", TopicId = 1, UserId = users[2].Id },
                new Post() { Name = "Sharing Creative PC Builds", TopicId = 1, UserId = users[1].Id },
                new Post() { Name = "NVME boot drive died .... how to recover?", TopicId = 1, UserId = users[1].Id },
                new Post() { Name = "Grinding on startup - HDD says caution", TopicId = 1, UserId = users[0].Id },
                new Post() { Name = "Is upgrading my Ram worth it?", TopicId = 1, UserId = users[5].Id },
                new Post() { Name = "partitioned drive, running out of space", TopicId = 1, UserId = users[1].Id },
                new Post() { Name = "PCCG do pickups anymore", TopicId = 1, UserId = users[3].Id },
                new Post() { Name = "TDP Question", TopicId = 1, UserId = users[3].Id },
                new Post() { Name = "LAG - 100+ tabs open - mostly graphs and twitter", TopicId = 1, UserId = users[0].Id },
                new Post() { Name = "replace dead gtx 970 with?", TopicId = 1, UserId = users[2].Id },
                new Post() { Name = "Best Benchmarking Apps", TopicId = 1, UserId = users[0].Id },
                new Post() { Name = "Next gen video cards", TopicId = 1, UserId = users[4].Id },
                new Post() { Name = "Dock for 2x Laptops/Desktop", TopicId = 2, UserId = users[2].Id },
                new Post() { Name = "The Headphone Thread", TopicId = 2, UserId = users[2].Id },
                new Post() { Name = "Force rotate webcam.", TopicId = 2, UserId = users[3].Id },
                new Post() { Name = "Dock for 2x Laptops/Desktop", TopicId = 2, UserId = users[3].Id },
                new Post() { Name = "Technices EAH-A800", TopicId = 2, UserId = users[4].Id },
                new Post() { Name = "Gaming Laptop", TopicId = 3, UserId = users[5].Id },
                new Post() { Name = "Laptop for work", TopicId = 3, UserId = users[5].Id },
                new Post() { Name = "Stuck on Boot Menu after SSD upgrade", TopicId = 3, UserId = users[1].Id },
                new Post() { Name = "Pixel buds pro", TopicId = 4, UserId = users[0].Id },
                new Post() { Name = "Best Game Controller for Windows", TopicId = 4, UserId = users[2].Id },
                new Post() { Name = "Best current email client", TopicId = 5, UserId = users[4].Id },
                new Post() { Name = "Windows 11", TopicId = 5, UserId = users[1].Id },
                new Post() { Name = "Removing One Drive", TopicId = 5, UserId = users[0].Id },
                new Post() { Name = "Movie Library Management And Cataloging", TopicId = 5, UserId = users[0].Id },
                new Post() { Name = "win7 to win10", TopicId = 5, UserId = users[3].Id },
                new Post() { Name = "Apple TV 4K (2017 and 2021)", TopicId = 6, UserId = users[5].Id },
                new Post() { Name = "Raspberry Pi", TopicId = 7, UserId = users[5].Id },
                new Post() { Name = "Switch to uefi", TopicId = 7, UserId = users[3].Id },
                new Post() { Name = "timeout when copying files between computers", TopicId = 7, UserId = users[4].Id },
                new Post() { Name = "Turing Pi 2", TopicId = 7, UserId = users[4].Id },
                new Post() { Name = "Telstra/Technicolor DJA0231 - root or boot?", TopicId = 8, UserId = users[2].Id },
                new Post() { Name = "ROOter OpenWRT router Adventures with 3G/4G/5G modems", TopicId = 8, UserId = users[0].Id },
                new Post() { Name = "Restarting Router & Modem", TopicId = 8, UserId = users[1].Id },
                new Post() { Name = "Dsl Wifi Router with 8x Gigabit Ports", TopicId = 8, UserId = users[5].Id },
                new Post() { Name = "Ubiquiti UniFi discussion", TopicId = 9, UserId = users[3].Id },
                new Post() { Name = "Netgear V7610 Setup", TopicId = 9, UserId = users[2].Id },
                new Post() { Name = "NTD to switch to another switch to router", TopicId = 9, UserId = users[4].Id },
                new Post() { Name = "Intercom phone for Teams calling", TopicId = 10, UserId = users[5].Id },
                new Post() { Name = "Siptalk - Customer Discussion", TopicId = 10, UserId = users[3].Id },
                new Post() { Name = "Crazytel MidYear 2021-WP Offers and Updates", TopicId = 10, UserId = users[5].Id },
                new Post() { Name = "Huge increase in scam calls lately?", TopicId = 10, UserId = users[4].Id },
                new Post() { Name = "VOIP with ABB suddenly 95% incoming are failing", TopicId = 10, UserId = users[4].Id },
                new Post() { Name = "\"Missed\" calls I did not make", TopicId = 10, UserId = users[0].Id },
                new Post() { Name = "Brute Force Attack", TopicId = 12, UserId = users[1].Id },
                new Post() { Name = "domain hosting", TopicId = 12, UserId = users[0].Id },
                new Post() { Name = "Can some other programmers have a look at my work", TopicId = 13, UserId = users[5].Id },
                new Post() { Name = "O'Reilly Subscription", TopicId = 13, UserId = users[1].Id },
                new Post() { Name = "HTTP/0.9 data", TopicId = 13, UserId = users[0].Id },
                new Post() { Name = "c#.net vs java", TopicId = 13, UserId = users[2].Id },
                new Post() { Name = "Reuse Telstra Smart Modem with another provider?", TopicId = 14, UserId = users[5].Id },
                new Post() { Name = "FTTP with a Asus AX86U. Exetel, Launtel or Telstra", TopicId = 14, UserId = users[1].Id },
                new Post() { Name = "Launtel vs SL vs ABB", TopicId = 14, UserId = users[0].Id },
                new Post() { Name = "ACCC NZ vs OZ Network speeds", TopicId = 15, UserId = users[0].Id },
                new Post() { Name = "Inter-country direct link (WAN?) latency", TopicId = 15, UserId = users[4].Id },
                new Post() { Name = "adsl2 or fixed wireless for latency", TopicId = 15, UserId = users[2].Id },
                new Post() { Name = "Fixed Wireless Discussion (NBN) ", TopicId = 16, UserId = users[4].Id },
                new Post() { Name = "Areas selected for NBN fibre extension program", TopicId = 16, UserId = users[4].Id },
                new Post() { Name = "NBN Rollout - Tarneit VIC (3TNI w/3WER)", TopicId = 16, UserId = users[3].Id }
            };

            List<Comment> commentList = new List<Comment>()
            {
                new Comment() { Content = "No big surprise that the 2070 will be the most popular one, followed by the 2080. Prices are certainly up there for Ti! We're certainly going to be paying top dollar for hitting those high-res and high-fps targets.", PostId = 1, UserId = users[2].Id },
                new Comment() { Content = "I'm honestly not surprised by the 2080Ti pricing.. It's basically the early Titan model we saw with Pascal which launched for $999 shortly after the 1080.", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "EVGA 2080 TI FTW3 Ultra For me thx", PostId = 1, UserId = users[2].Id },
                new Comment() { Content = "What is the founder's edition as opposed to the normal 2080ti?", PostId = 1, UserId = users[2].Id },
                new Comment() { Content = "Founders edition is a line of cards sold by nVidia directly. They are basically now their own partner board manufacturer. They have no special features other than the cooler being designed by nVidia themselves.", PostId = 1, UserId = users[3].Id },
                new Comment() { Content = "Will these new cards improve my HTC Vive ?", PostId = 1, UserId = users[3].Id },
                new Comment() { Content = "There are no new VR-specific rendering features. There is a VR-Link connector but unless your headset already has a VR-Link connection it may not be useful. It's unclear if any of the existing ones will be adapted for the simpler connection.", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "Yes with virtual link usb c – faster rendering for vr", PostId = 1, UserId = users[3].Id },
                new Comment() { Content = "Depending what card you have now you could crank the super sampling more, I can do 1.3x on my 1070", PostId = 1, UserId = users[4].Id },
                new Comment() { Content = "Big ooof at the pricing. We expecting the non-refs to be cheaper, similar or more exy than the FE models?", PostId = 1, UserId = users[3].Id },
                new Comment() { Content = "According to Gigabyte's released pricing, it will be about the same", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "Will this make high FPS (up to 144Hz) 4k worth it? I have a 1080 at the moment which isn't always managing 60Hz (that's all I need at the moment due to monitor limit). Was going to build a new rig from the ground up when something that's a worthwhile upgrade for 4k is released. At least releasing the Ti version at the same time makes it a little less of a risk of significant hardware improvements within a year.", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "That information is not yet available. We won't know until the review embargo lifts sometime between now and Septermber 20.", PostId = 1, UserId = users[0].Id },
                new Comment() { Content = "I guess we'll wait and see... Once I decide, I wonder if i'll be waiting as long for the 2080ti as I did for the EVGA 1080 SC... it was a good few months of prowling supplier sites before I actually had one in my hands...", PostId = 1, UserId = users[3].Id },
                new Comment() { Content = "It feels like that availability will be better this time, but yeah hard to say in advance. I think there won't be as much as a rush for stock because the prices are higher this time around.", PostId = 1, UserId = users[5].Id },
                new Comment() { Content = "Agreed, they're also releasing the 2070, 2080 and 2080ti. From memory I think they did the 1080 then waited before the 1070 then at least a year before the 1080ti so people wanting the 'newest' were given one option. I think the perceived value of these cards will also be a lot lower. It doesn't seem like as much of a leap, regardless of how big a jump there actually was between the 980ti and the 1080. At least the perceived value isn't quite as good for some reason IMO.", PostId = 1, UserId = users[3].Id },
                new Comment() { Content = "I have a GTX 1080 gigabyte g1 gaming", PostId = 1, UserId = users[0].Id },
                new Comment() { Content = "So US pricing is $1199 USD for the 2080 Ti FE and $799 USD for the 2080 FE. That is $400 USD difference. AU pre-order pricing is $1899 AUD for the 2080 Ti FE and $1199 AUD for the 2080 FE. That is a whopping $700 AUD($515 USD) difference. Wait, what ? ", PostId = 1, UserId = users[2].Id },
                new Comment() { Content = "Sweet, have added the 2070 price", PostId = 1, UserId = users[5].Id },
                new Comment() { Content = "Haven't read thoroughly through all the news but these are looking like a pretty good jump forward in performance?", PostId = 1, UserId = users[3].Id },
                new Comment() { Content = "So probably looking at around $800 for somewhere like MSY for the 2070... about the same as what the 1070s were when they were first released, maybe marginally higher. Will be very interested to see the benchmarks between 10 vs 20 series. Wouldn't mind upgrading my 1070, it's having a hard time maintaining 120fps stable in BF1 once all the destruction takes place.", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "I feel like the actual performance jump will be disappointing as they only talked about how the 20 series cards can do so much ray tracing (GIGA RAYS YO) but not any real game performance comparisons with the 10 series cards.", PostId = 1, UserId = users[2].Id },
                new Comment() { Content = "Hrmm yeah im not gonna get too pumped up just yet. RTX-OPS...I worry when companies start redefining benchmarks. Will put the wallet back and wait for reviews me thinks", PostId = 1, UserId = users[3].Id },
                new Comment() { Content = "It does strike me as a bit odd that they're keeping tight lipped about performance benchmarks. At this stage I'm not really feeling the ray tracing marketing hype... reeks of Nvidia GameWorks to me. All I really care about are those sweet, sweet frames.", PostId = 1, UserId = users[2].Id },
                new Comment() { Content = "Nearly a $300 premium based on the exchange rate. Absolutely disgusting $1600 bucks is insane but not bat sh*t", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "Its a much larger die size, so it makes sense that the price has increased. ~60% larger die with a ~71% increase in price.", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "It's not just you, and it's not just computer hardware. Basically everything on the market is getting more expensive, and wages are barely growing. Good times.", PostId = 1, UserId = users[5].Id },
                new Comment() { Content = "So basically pascal can still render games with RTX on but just slower to a point that its unplayable", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "Checked my Umart history from when I bought my 1080 back in Sept 2016 and paid $1150, so the 2080 is actually bang on price wise. Think I'll skip this gen of cards though, still managing to play new releases very well.", PostId = 1, UserId = users[2].Id },
                new Comment() { Content = "I paid around $1250 for a launch GTX 1080.", PostId = 1, UserId = users[3].Id },
                new Comment() { Content = "Since I sold my 970 SLI AGES ago.. I've been rocking, nothing. May jump in again if there is an actual improvement and not just lip service / new \"definition\" on what to measure.", PostId = 1, UserId = users[2].Id },
                new Comment() { Content = "If you haven't had a GPU for ages, I doubt there is anything you'd be interested in now.", PostId = 1, UserId = users[0].Id },
                new Comment() { Content = "Until AMD actually pull their finger out and release something competitive, nvidia will continue to charge what they like.", PostId = 1, UserId = users[0].Id },
                new Comment() { Content = "I'm looking forward to NVlink, could be dud. Any info?", PostId = 1, UserId = users[0].Id },
                new Comment() { Content = "Based on the RTX8000 capabilities, it allows 100GB/s transfer which is fast enough now to let the GPUs combine their memory together.", PostId = 1, UserId = users[0].Id },
                new Comment() { Content = "Hopefully, it translates into better multi-GPU scaling and better frame time. I'm quite happy with SLI at the moment but more is better I guess.", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "Nearest I can find, the original SLI bridges had between 1-4GB/s of bandwidth, so it's definitely an upgrade. But the question is what nVidia does with the extra bandwidth. If they do allow you to double the memory size with 2 cards, it would mean that 2x GTX 2080 16GB DDR6 would be a good 4K gaming solution(Assuming compute scales)", PostId = 1, UserId = users[2].Id },
                new Comment() { Content = "Pretty brutal estimate on performances from Steve HardwareUnboxed. Seriously, a 10% performance boost Pascal Vs Turing, is that even possible? I reckon he's wrong.", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "Hmm a stop gap til 7nm", PostId = 1, UserId = users[3].Id },
                new Comment() { Content = "Seems a bit whacked – and would fly in face of that one non ray tracing benchmark we do have – but we'll know soon i guess.", PostId = 1, UserId = users[1].Id },
                new Comment() { Content = "Don't forget we have 10% GST. Brings it up to $1760 then you have import taxes and shipping to $1899.", PostId = 1, UserId = users[2].Id },
                new Comment() { Content = "Don’t forget that USA prices don’t include sales tax. California for example has 8.25% sales tax. Still a bit of a gap, but not that huge. Consumer warranties are likely better here too. I’m not defending NVIDIA’s high prices, but so many people forget that US advertised prices don’t include tax.", PostId = 1, UserId = users[0].Id },
                new Comment() { Content = "FE cards U$1200 which makes sense with the price of AU$1900 (US$1200 = AU$1600, add 5% duty and 10% GST and AU$1899 isn't far off... Partner cards are going to be US$1000 so they should be AU$1500-1600 methinks...", PostId = 1, UserId = users[0].Id },
                new Comment() { Content = "Based on the hardware specs alone, I am expecting around 10 to 20 percent performance improvement in frames for current/older games that do not leverage the new cores. Heavy use of ray tracing will obviously slaughter performance for the added pretty, and even significantly more so if you do not have the hardware to accelerate it.", PostId = 1, UserId = users[4].Id },
                new Comment() { Content = "In most states/areas of the US, prices are advertised tax exclusive and is added at the checkout. So you shouldn't be paying US sales tax.", PostId = 1, UserId = users[2].Id }
            };

            await _context.Subforums.AddRangeAsync(subforumList);
            await _context.Topics.AddRangeAsync(topicList);
            await _context.Posts.AddRangeAsync(postList);
            await _context.Comments.AddRangeAsync(commentList);

            await _context.SaveChangesAsync();
        }
    }
}
