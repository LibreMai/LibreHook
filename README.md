# SgHook

sega unity游戏运行时Hook工具

## 可选编译宏

如果给亲友游玩，请避免添加以下几个编译宏
```
OVERRIDEPOWERON NETPACK AUTOPLAY SENSITIVE
```

## 優點
運行時`Hook`，可以不對`Sinmai.exe`、`Assembly-CSharp.dll`等PE文件脫殼  
對於未知的版本，可能可以做到跨版本兼容，即使在HtPec无解的情况下，依然可以运行很多功能   
協同編寫Patch方便，無需對dll進行任何修改

## 功能

詳見`sghook.yaml`

- 打印網絡日志
- 繞過環境檢測
- 運行時内存轉儲
- 嗅探密碼字段
- 判定調整、音量调整
- 自定义攝像頭檢查
- 鎖定剩餘幣數
- AutoPlay功能
- 恢复删除的输入


## 使用方式

- 將`MelonLoader`裝入Sinmai.exe
- 將該項目構建完放入`Mods`
- 將項目依賴dll（可在Nuget以及輸出文件找到）放入UserLib
- 複製倉庫根目錄`sghook.yaml`配置文件至`Package`根目錄
- 在`sghook.yaml`中調整需要變更的參數
- 開玩！