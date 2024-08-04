# CongressionalTradeScanner

> Please Mrs. Pelosi just a crumb of Alpha


## Known issues/improvements
- Apparantly, the senate is much more tech focused with their data and provide clear XML files which we can use to generate the commitee/subcommitee structure. The house, on the other hand, provides no such files so the structure is built from scraping the site. This could lead to issues down the line if the site structure is ever updated. Luckily, the government is not known for their love of web development.
- Eventually, I want to assign sectors on the commitees, this way we can attempt some kind of GICS Decomposition on the trades alerts to further filter notifications. For example, a rep sitting on a tech commitee trading tech stocks is much more significant than them trading Johnson & Johnson.
