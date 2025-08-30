import json
from functools import reduce
import os

# -----------------------------
# Lagrange interpolation math
# -----------------------------
def lagrange_interpolate(x, x_s, y_s, p=None):
    """
    Given (x_s, y_s), interpolate the polynomial and evaluate at x.
    If p is given, all operations are mod p (finite field).
    """
    k = len(x_s)
    assert k == len(set(x_s)), "x values must be distinct"

    def PI(vals):  # product of list
        return reduce(lambda a, b: a * b, vals, 1)

    total = 0
    for i in range(k):
        xi, yi = x_s[i], y_s[i]

        # Compute L_i(x) = ∏ (x - xj) / (xi - xj) for j != i
        terms = [(x - x_s[m]) * pow(xi - x_s[m], -1, p) if p else (x - x_s[m])/(xi - x_s[m])
                 for m in range(k) if m != i]

        li = PI(terms)  # Lagrange basis polynomial value at x

        if p:
            total += yi * li
            total %= p
        else:
            total += yi * li
    return total

# -----------------------------
# Load testcase2.json
# -----------------------------
desktop_path = os.path.join(os.path.expanduser("~"), "Desktop", "testcase2.json")

with open(desktop_path, "r") as f:
    data = json.load(f)

n = data["keys"]["n"]
k = data["keys"]["k"]

# Convert shares into (x, y) points
shares = []
for key, entry in data.items():
    if key == "keys":
        continue
    base = int(entry["base"])
    value_str = entry["value"]
    y = int(value_str, base)  # convert from base → integer
    x = int(key)              # share index is the x value
    shares.append((x, y))

# Sort shares and pick first k
shares = sorted(shares, key=lambda t: t[0])[:k]

x_s = [s[0] for s in shares]
y_s = [s[1] for s in shares]

# -----------------------------
# Find the secret (f(0))
# -----------------------------
secret = lagrange_interpolate(0, x_s, y_s)

print("Chosen shares (x, y):", shares)
print("Recovered Secret (f(0)) =", int(secret))
