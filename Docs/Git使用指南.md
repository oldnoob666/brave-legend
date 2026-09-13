# 本项目的 Git 使用指南

Git 用来给项目保存可查阅、可恢复的版本。一次 `commit`（提交）就是一次有说明的存档。保存 Unity 文件不会自动产生 Git 提交。

仓库位于 `D:\unity01\My project (2)`，默认分支为 `main`。当前只在本机使用，没有连接远程仓库。

## 哪些内容进入版本管理

- `Assets/`：脚本、场景、图片和对应的 `.meta` 文件。
- `Packages/`：包配置和依赖锁定文件。
- `ProjectSettings/`：Unity 项目设置。
- `Docs/`、`Backups/`：现有文档和手动备份。
- `.gitignore`、`.gitattributes`、`.vsconfig`：仓库和开发环境配置。

`Library/`、`Temp/`、`Logs/`、`obj/`、`UserSettings/`、`.vs/` 以及自动生成的 `.sln`、`.csproj` 文件被忽略，仍留在本机。Unity 会按需重新生成缓存和工程文件。不要忽略或单独删除 `.meta` 文件，它们负责保持资源引用。

项目使用 Unity `2022.3.59f1c1`，已启用文本序列化及 Visible Meta Files。

## 第一次使用

打开 PowerShell，进入项目：

```powershell
Set-Location -LiteralPath 'D:\unity01\My project (2)'
```

如需设置或更改提交身份，替换下方示例。配置只影响当前仓库，无需真实邮箱也能在本地提交：

```powershell
git config --local user.name "你的名字"
git config --local user.email "you@example.com"
```

新身份用于之后的提交，已有提交保留原记录。

## 每次完成一个小功能后

先在 Unity 保存场景、资源以及脚本，等待资源导入完成，然后依次运行：

```powershell
git status
git diff
git add .
git diff --cached --stat
git commit -m "完成背包物品拖拽"
git status
```

`status` 查看修改；`diff` 查看已跟踪文件尚未暂存的具体变化（新文件需直接检查）；`add .` 把当前目录下所有未被忽略的改动加入暂存区；`diff --cached --stat` 查看本次提交范围；`commit` 生成存档。提交说明换成你实际完成的内容。最后出现 `nothing to commit, working tree clean` 表示所有改动已提交。

想只提交某些文件，用 `git add "具体文件路径"` 替代 `git add .`。Unity 资源新增或移动时，一起提交相关 `.meta`。如果提交后又修改文件，需要重新 `add` 才能放入下一次提交。

## 查看历史

```powershell
git log --oneline --decorate -10
git show --stat HEAD
```

第一条查看最近十次提交；第二条查看最新一次提交的文件概览。`HEAD` 表示当前提交。

## 开分支尝试新功能

先把当前工作提交干净，再创建分支：

```powershell
git switch -c feature/new-inventory
```

在分支上修改、保存，并照常 `add` 和 `commit`。满意后合并回主分支：

```powershell
git switch main
git merge feature/new-inventory
```

分支可以保留。切换分支或恢复场景前，先保存并关闭 Unity，避免编辑器把内存中的旧状态写回磁盘，之后重新打开项目。遇到合并冲突时，先用 `git status` 查看并解决，再提交；不确定时可以请我协助。

## 撤销时区分三种情况

**只是误加入暂存区，文件修改要保留：**

```powershell
git restore --staged "Assets/某个脚本.cs"
```

**确定丢弃某个已跟踪文件的未提交修改，恢复到当前提交：**

```powershell
git restore --source=HEAD --staged --worktree -- "Assets/某个脚本.cs"
```

请把示例路径换成真实文件。第二条会覆盖该文件的未提交内容，Git 无法保证找回这些内容；不确定时先复制备份。恢复资源时还要考虑关联的 `.meta`。

**撤销已经提交的最新一次修改，并留下撤销记录：**

```powershell
git revert --no-edit HEAD
```

先确保工作区干净。这条用于普通提交；初始提交、合并提交或发生冲突时，先确认处理方式，不要直接照抄。

## 本地仓库与备份

提交历史保存在项目根目录的隐藏 `.git/` 文件夹中。不要删除它；复制项目并希望保留历史时，也要复制它。本地 Git 可以找回已提交版本，但电脑磁盘损坏仍可能丢失全部记录，重要项目应另做异盘备份，或之后连接远程仓库。
