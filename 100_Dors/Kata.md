# 🧩 100 Doors Problem

There are **100 doors** in a row, all initially **closed**.
You make **100 passes** through the doors.
On each pass, you toggle the state of certain doors based on the pass number:

---

## 🔄 Toggling Rules

* **Pass 1:**
  Visit **every door** → open all of them.

* **Pass 2:**
  Visit **every 2nd door** (2, 4, 6, …) → toggle their state (open → closed, closed → open).

* **Pass 3:**
  Visit **every 3rd door** (3, 6, 9, …) → toggle their state.

* Continue this pattern until:

* **Pass 100:**
  Visit only **door #100** and toggle it.

---

## ❓ Goal

After completing **all 100 passes**, determine the **final state of each door**.

### Output Format

* Print the 100 doors in a **single string**:

  * `@` → door is **open**
  * `#` → door is **closed**

Example of what the **first six doors** might look like:

```
@@##@@##
```

---
