# 指南与 SourceGit v2026.19 对照

2026-09-13 再次完整读取 `../../Docs/Git使用指南.md` 后核对。用户已明确改为先使用现成 SourceGit、保留源码，不继续补齐定制要求。下表“有”指在发行版对应源码中查到入口及实现，不等于所有功能均已进行破坏性操作实测。

代码路径均相对 `../vendor/release-source/sourcegit-2026.19/src/`。

| 指南功能 | 覆盖情况 | 程序入口 | 代码 |
| --- | --- | --- | --- |
| 进入仓库目录 | 有，GUI 替代 Set-Location | 打开仓库、最近仓库；本项目快捷方式 | ViewModels/Launcher.cs、Commands/QueryRepositoryRootPath.cs |
| 本仓库姓名 | 有 | 配置本仓库 | ViewModels/RepositoryConfigure.cs、Commands/Config.cs |
| 本仓库邮箱 | 有 | 配置本仓库 | ViewModels/RepositoryConfigure.cs、Commands/Config.cs |
| status 和干净状态检查 | 有 | 本地更改、分支状态 | Commands/QueryLocalChanges.cs、ViewModels/WorkingCopy.cs |
| diff 未暂存 | 有 | 未暂存列表选中文件 | ViewModels/DiffContext.cs、Commands/Diff.cs |
| add . 全部暂存 | 有 | 暂存所有 | ViewModels/WorkingCopy.cs、Commands/Add.cs |
| add 指定文件 | 有 | 选择一个或多个文件，暂存(add) | ViewModels/WorkingCopy.cs、Commands/Add.cs |
| diff --cached --stat 暂存概览 | 功能对应，非原样命令输出 | 已暂存文件列表及 Diff；不是单独 stat 按钮 | ViewModels/WorkingCopy.cs、ViewModels/DiffContext.cs |
| add → commit -m → status | 有 | 暂存、填消息、提交，自动刷新 | ViewModels/WorkingCopy.cs、Commands/Commit.cs |
| log --oneline --decorate -10 | 有，可看更多历史，非固定十条命令 | 历史记录 | Commands/QueryCommits.cs、ViewModels/Histories.cs |
| show --stat HEAD | 功能对应，非原样命令输出 | 选择 HEAD 提交，查看文件与 Diff | Commands/QuerySingleCommit.cs、ViewModels/CommitDetail.cs |
| switch -c 创建并切换 | 有等价流程，实际使用 checkout | 创建分支，选择检出 | ViewModels/CreateBranch.cs、Commands/Branch.cs、Commands/Checkout.cs |
| switch 分支 | 有等价流程，实际使用 checkout | 本地分支菜单／双击分支 | ViewModels/Repository.cs、Commands/Checkout.cs |
| switch main → merge 功能分支 | 有，两步完成 | 先切换 main，再合并选定功能分支 | Commands/Checkout.cs、Commands/Merge.cs |
| restore --staged | 有等价取消暂存，实际使用按路径 reset | 从暂存中移除、从暂存区移除所有 | ViewModels/WorkingCopy.cs、Commands/Reset.cs |
| restore --source=HEAD --staged --worktree -- 文件 | 两步等价流程；没有此精确命令的一键入口 | 先取消目标文件暂存，再丢弃目标文件工作区更改 | Commands/Reset.cs、Commands/Discard.cs、Commands/Restore.cs |
| revert --no-edit HEAD | 有；上游同时支持合并提交主线参数 | 最新提交右键“回滚此提交”，选择自动提交 | ViewModels/Revert.cs、Commands/Revert.cs |
| 冲突 status → 人工编辑 → add → commit | 有 | 冲突文件、外部编辑器／合并工具、暂存、提交 | ViewModels/WorkingCopy.cs、Commands/QueryConflictFileState.cs、Commands/MergeTool.cs |
| .meta 与资源一起提交 | 人工配对 | 多选资源和相关 .meta | 无自动 Unity 配对逻辑 |
| Unity 缓存忽略 | 使用已有 Git 规则 | .gitignore 生效，缺少专项检查按钮 | 项目现有 .gitignore；没有本次定制功能 |
| 保存并关闭 Unity | 人工操作 | README 提示 | 不属于 Git 命令 |
| 保留 .git、异盘备份 | 人工操作 | README 提示 | 不属于 Git 命令 |

## 额外要求的现成功能

| 功能 | 对应位置 |
| --- | --- |
| Fetch / Pull / Push | 顶部工具栏；Commands/Fetch.cs、Pull.cs、Push.cs |
| Remote 查看、新增、编辑、删除 | 远程列表；ViewModels/AddRemote.cs、EditRemote.cs、DeleteRemote.cs |
| 上游跟踪分支 | 分支上下文菜单；Commands/Branch.cs |
| 本地／远程分支、指定起点创建、删除、Merge | 分支列表；Commands/QueryBranches.cs、Branch.cs、Merge.cs |
| Stash 创建、消息、列表、应用、应用后删除、删除 | 贮藏列表；Commands/Stash.cs、QueryStashes.cs；ViewModels/ApplyStash.cs、DropStash.cs |
| 已暂存／提交／两个提交 Diff、复制哈希 | 本地更改与历史记录；Commands/Diff.cs、CompareRevisions.cs |
| Git 命令日志 | 工具栏“查看命令日志”；ViewModels/ViewLogs.cs、CommandLog.cs |
| Git 统一执行和异步操作 | Commands/Command.cs，各 Commands 实现及 ViewModels async 调用 |
| 中文和便携配置 | Resources/Locales/zh_CN.axaml、ViewModels/Preferences.cs、Native/Windows.cs |

## 与原自研规格的差异

1. C# + Avalonia / .NET 10，不是 WPF / .NET 8；使用官方已编译便携包。
2. SourceGit 有危险操作弹窗，但没有统一的“显示完整命令、解释影响、两次确认”机制，命令日志不是每项操作事前预览。
3. 未加 Unity `.gitignore` 专项检查按钮或 `.meta` 自动配对。
4. 取消暂存、切换分支等采用功能等价的 Git 命令；精确恢复到 HEAD 是两步流程。
5. 暂存及提交文件概览由 GUI 展示，不保证原样显示指南的 `--stat` 文本。
6. SourceGit 可经确认创建空提交，未按原规格完全禁止；原始 Git 错误不保证全部转换成中文解释。
7. 本次仅完成下载、配置、源码核对和只读桌面冒烟检查，未跑原定制计划的完整 Git 操作测试矩阵。

这些差异已在选择现成方案前向用户说明，后续定制可从保留的源码继续。
