---
description: Merge code từ dev_api_linh_20260207 sang main và deploy
---

## Workflow: Merge dev branch sang main

### Bước 1: Đảm bảo đang ở nhánh dev và code đã commit + push
// turbo
```
git status
```

### Bước 2: Commit nếu có thay đổi chưa commit
```
git add .
git commit -m "mô tả thay đổi"
```

### Bước 3: Push code lên nhánh dev
```
git push origin dev_api_linh_20260207
```

### Bước 4: Checkout sang main và pull mới nhất
// turbo
```
git checkout main
git pull origin main
```

### Bước 5: Merge dev vào main
```
git merge dev_api_linh_20260207
```

### Bước 6: Push main lên GitHub (sẽ tự trigger deploy)
```
git push origin main
```

### Bước 7: Quay lại nhánh dev để tiếp tục code
// turbo
```
git checkout dev_api_linh_20260207
```
