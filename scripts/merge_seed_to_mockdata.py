import os


BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DOCS_DIR = os.path.join(BASE_DIR, "docs")

FAKE = os.path.join(DOCS_DIR, "seed_fake_data.sql")
SEED300 = os.path.join(DOCS_DIR, "seed_books_authors_300.sql")
OUT = os.path.join(DOCS_DIR, "MockData.sql")


def read(path: str) -> str:
    with open(path, "r", encoding="utf-8") as f:
        return f.read()


def normalize(s: str) -> str:
    return s.replace("\r\n", "\n")


def main() -> None:
    fake = normalize(read(FAKE))
    s300 = normalize(read(SEED300))

    # Strip headers & SET NOCOUNT from both sources
    def body_after_set_nocount(src: str) -> str:
        marker = "SET NOCOUNT ON;"
        idx = src.find(marker)
        if idx == -1:
            return src
        return src[idx + len(marker) :].lstrip()

    fake_body = body_after_set_nocount(fake)
    s300_body = body_after_set_nocount(s300)

    # Remove Categories block (1-25) from second file to avoid PK duplicates
    cat_header = "-- ==================== CATEGORIES"
    next_after_cat = "-- ==================== PUBLISHERS"
    cat_start = s300_body.find(cat_header)
    if cat_start != -1:
        cat_end = s300_body.find(next_after_cat, cat_start)
        if cat_end != -1:
            s300_body = s300_body[:cat_start] + s300_body[cat_end:]

    header = "\n".join(
        [
            "-- ============================================================",
            "-- BookStation MockData.sql (merged)",
            "-- Combined: docs/seed_fake_data.sql + docs/seed_books_authors_300.sql",
            "-- Run in SSMS 2022. Order respects foreign keys.",
            "-- If OrderItems has column 'VariantId' instead of 'BookVariantId', replace in OrderItems section.",
            "-- ============================================================",
            "SET NOCOUNT ON;",
            "",
        ]
    )

    out_text = (
        header
        + fake_body.strip()
        + "\n\n-- ==================== EXTRA DATA (~300 authors & books) ====================\n\n"
        + s300_body.strip()
        + "\n\nPRINT 'MockData.sql (combined) completed.';\n"
    )

    with open(OUT, "w", encoding="utf-8") as f:
        f.write(out_text)

    print("Written", OUT)


if __name__ == "__main__":
    main()

# Merge docs/seed_fake_data.sql and docs/seed_books_authors_300.sql into docs/MockData.sql
import re
import os

DOCS = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "docs")
FAKE = os.path.join(DOCS, "seed_fake_data.sql")
SEED300 = os.path.join(DOCS, "seed_books_authors_300.sql")
OUT = os.path.join(DOCS, "MockData.sql")

def read(path):
    with open(path, "r", encoding="utf-8") as f:
        return f.read()

def extract(s, start_marker, end_marker):
    """Extract block from first line containing start_marker to line before end_marker (or end)."""
    lines = s.replace("\r\n", "\n").split("\n")
    start_i = None
    end_i = None
    for i, line in enumerate(lines):
        if start_marker in line:
            start_i = i
        if start_i is not None and end_marker and end_marker in line and i > start_i:
            end_i = i
            break
    if start_i is None:
        return None
    if end_i is None:
        end_i = len(lines)
    return "\n".join(lines[start_i:end_i])

def extract_block_by_header(content, header_start, next_header_start):
    """Get full block from header line until (excluding) next header. content is raw string."""
    c = content.replace("\r\n", "\n")
    i = c.find(header_start)
    if i == -1:
        return None
    j = c.find(next_header_start, i + 1)
    if j == -1:
        return c[i:]
    return c[i:j].strip()

def extract_to_end(s, start_marker):
    """From start_marker to end of file."""
    idx = s.find(start_marker)
    if idx == -1:
        return None
    return s[idx:]

def fix_author_dates(block):
    """Fix unquoted dates in Authors INSERT: 1960-09-09 -> '1960-09-09'."""
    return re.sub(r"(?<!')(\b\d{4}-\d{2}-\d{2}\b)(?!')", r"'\1'", block)

def main():
    fake = read(FAKE)
    s300 = read(SEED300)

    out = []
    out.append("-- ============================================================")
    out.append("-- BookStation MockData.sql (merged)")
    out.append("-- Combined: seed_fake_data.sql + seed_books_authors_300.sql")
    out.append("-- Run in SSMS 2022. Order respects foreign keys.")
    out.append("-- If OrderItems has column 'VariantId' instead of 'BookVariantId', replace in OrderItems section.")
    out.append("-- ============================================================")
    out.append("SET NOCOUNT ON;")
    out.append("")

    # 1) Categories 1-25 from seed_books_authors_300
    out.append("-- ==================== CATEGORIES (1-25) ====================")
    cat_block = extract_to_end(s300, "-- ==================== CATEGORIES").split("-- ==================== PUBLISHERS")[0].strip()
    if cat_block.startswith("-- ==================== CATEGORIES"):
        first_line = cat_block.split("\n")[0]
        cat_block = cat_block[len(first_line):].lstrip()
    out.append(cat_block)
    out.append("")

    # 2) Publishers: 5 from fake + 50 from 300 -> one INSERT with 55 rows
    out.append("-- ==================== PUBLISHERS (55: 5 + 50) ====================")
    pub_fake = extract_block_by_header(fake, "INSERT INTO Publishers", "-- ==================== AUTHORS")
    pub_300 = extract_block_by_header(s300, "INSERT INTO Publishers", "-- ==================== AUTHORS")
    if pub_fake and pub_300:
        pub_fake_vals = pub_fake.replace("INSERT INTO Publishers (Id, Name, Description, Address, Website, LogoUrl, IsActive, CreatedAt, UpdatedAt) VALUES\n", "").strip()
        for suf in [";", "\n;"]:
            if pub_fake_vals.endswith(suf):
                pub_fake_vals = pub_fake_vals[:-len(suf)].strip()
        pub_300_vals = pub_300.replace("INSERT INTO Publishers (Id, Name, Description, Address, Website, LogoUrl, IsActive, CreatedAt, UpdatedAt) VALUES\n", "").strip()
        for suf in [";", "\n;"]:
            if pub_300_vals.endswith(suf):
                pub_300_vals = pub_300_vals[:-len(suf)].strip()
        out.append("INSERT INTO Publishers (Id, Name, Description, Address, Website, LogoUrl, IsActive, CreatedAt, UpdatedAt) VALUES")
        out.append(pub_fake_vals + ",")
        out.append(pub_300_vals)
        out.append(";")
    out.append("")

    # 3) Authors: 12 from fake + 300 from 300 (fix dates in 300)
    out.append("-- ==================== AUTHORS (312: 12 + 300) ====================")
    auth_fake = extract_block_by_header(fake, "INSERT INTO Authors", "-- ==================== USERS")
    auth_300 = extract_block_by_header(s300, "INSERT INTO Authors", "-- ==================== BOOKS")
    if auth_fake and auth_300:
        auth_fake_vals = auth_fake.replace("INSERT INTO Authors (Id, FullName, Bio, DateOfBirth, DiedDate, Address, Country, PhotoUrl, UserId, CreatedAt, UpdatedAt) VALUES\n", "").strip()
        for suf in [";", "\n;"]:
            if auth_fake_vals.endswith(suf):
                auth_fake_vals = auth_fake_vals[:-len(suf)].strip()
        auth_300_vals = auth_300.replace("INSERT INTO Authors (Id, FullName, Bio, DateOfBirth, DiedDate, Address, Country, PhotoUrl, UserId, CreatedAt, UpdatedAt) VALUES\n", "").strip()
        for suf in [";", "\n;"]:
            if auth_300_vals.endswith(suf):
                auth_300_vals = auth_300_vals[:-len(suf)].strip()
        auth_300_vals = fix_author_dates(auth_300_vals)
        out.append("INSERT INTO Authors (Id, FullName, Bio, DateOfBirth, DiedDate, Address, Country, PhotoUrl, UserId, CreatedAt, UpdatedAt) VALUES")
        out.append(auth_fake_vals + ",")
        out.append(auth_300_vals)
        out.append(";")
    out.append("")

    # 4) Users from fake
    out.append(extract_to_end(fake, "-- ==================== USERS").split("-- ==================== BOOKS")[0].strip())
    out.append("")

    # 5) Books: 20 from fake + 300 from 300 (21-320)
    out.append("-- ==================== BOOKS (320: 1-20 + 21-320) ====================")
    out.append("SET IDENTITY_INSERT Books ON;")
    books_fake = extract(fake, "INSERT INTO Books (Id, Title", ";\nSET IDENTITY_INSERT Books OFF")
    books_300 = extract(s300, "INSERT INTO Books (Id, Title", ";\nSET IDENTITY_INSERT Books OFF")
    if books_fake and books_300:
        bf = books_fake.replace("INSERT INTO Books (Id, Title, ISBN, Description, Language, PublishYear, PublisherId, SellerId, Status, CoverImageUrl, PageCount, CoAuthors, Translators, CreatedAt, UpdatedAt) VALUES\n", "").strip()
        if bf.endswith(";"):
            bf = bf[:-1].strip()
        b3 = books_300.replace("INSERT INTO Books (Id, Title, ISBN, Description, Language, PublishYear, PublisherId, SellerId, Status, CoverImageUrl, PageCount, CoAuthors, Translators, CreatedAt, UpdatedAt) VALUES\n", "").strip()
        if b3.endswith(";"):
            b3 = b3[:-1].strip()
        out.append("INSERT INTO Books (Id, Title, ISBN, Description, Language, PublishYear, PublisherId, SellerId, Status, CoverImageUrl, PageCount, CoAuthors, Translators, CreatedAt, UpdatedAt) VALUES")
        out.append(bf + ",")
        out.append(b3)
        out.append(";")
    out.append("SET IDENTITY_INSERT Books OFF;")
    out.append("")

    # 6) BookVariants: 40 from fake + 600 from 300
    out.append("-- ==================== BOOK VARIANTS (640: 1-40 + 41-640) ====================")
    out.append("SET IDENTITY_INSERT BookVariants ON;")
    vf = extract(fake, "INSERT INTO BookVariants (Id, BookId", ";\nSET IDENTITY_INSERT BookVariants OFF")
    v3 = extract(s300, "INSERT INTO BookVariants (Id, BookId", ";\nSET IDENTITY_INSERT BookVariants OFF")
    if vf and v3:
        vf_vals = vf.replace("INSERT INTO BookVariants (Id, BookId, VariantName, Price, PriceCurrency, OriginalPrice, OriginalPriceCurrency, WeightGrams, SKU, IsAvailable, CreatedAt, UpdatedAt) VALUES\n", "").strip()
        if vf_vals.endswith(";"):
            vf_vals = vf_vals[:-1].strip()
        v3_vals = v3.replace("INSERT INTO BookVariants (Id, BookId, VariantName, Price, PriceCurrency, OriginalPrice, OriginalPriceCurrency, WeightGrams, SKU, IsAvailable, CreatedAt, UpdatedAt) VALUES\n", "").strip()
        if v3_vals.endswith(";"):
            v3_vals = v3_vals[:-1].strip()
        out.append("INSERT INTO BookVariants (Id, BookId, VariantName, Price, PriceCurrency, OriginalPrice, OriginalPriceCurrency, WeightGrams, SKU, IsAvailable, CreatedAt, UpdatedAt) VALUES")
        out.append(vf_vals + ",")
        out.append(v3_vals)
        out.append(";")
    out.append("SET IDENTITY_INSERT BookVariants OFF;")
    out.append("")

    # 7) Inventories: 40 from fake + 600 from 300
    out.append("-- ==================== INVENTORIES (640) ====================")
    out.append("SET IDENTITY_INSERT Inventories ON;")
    inv_f = extract(fake, "INSERT INTO Inventories (Id, BookVariantId", ";\nSET IDENTITY_INSERT Inventories OFF")
    inv_3 = extract(s300, "INSERT INTO Inventories (Id, BookVariantId", ";\nSET IDENTITY_INSERT Inventories OFF")
    if inv_f and inv_3:
        inv_f_v = inv_f.replace("INSERT INTO Inventories (Id, BookVariantId, QuantityInStock, ReservedQuantity, CreatedAt, UpdatedAt) VALUES\n", "").strip()
        if inv_f_v.endswith(";"):
            inv_f_v = inv_f_v[:-1].strip()
        inv_3_v = inv_3.replace("INSERT INTO Inventories (Id, BookVariantId, QuantityInStock, ReservedQuantity, CreatedAt, UpdatedAt) VALUES\n", "").strip()
        if inv_3_v.endswith(";"):
            inv_3_v = inv_3_v[:-1].strip()
        out.append("INSERT INTO Inventories (Id, BookVariantId, QuantityInStock, ReservedQuantity, CreatedAt, UpdatedAt) VALUES")
        out.append(inv_f_v + ",")
        out.append(inv_3_v)
        out.append(";")
    out.append("SET IDENTITY_INSERT Inventories OFF;")
    out.append("")

    # 8) BookAuthors: fake + 300
    out.append("-- ==================== BOOK AUTHORS ====================")
    ba_f = extract_to_end(fake, "INSERT INTO BookAuthors").split("-- ==================== BOOK CATEGORIES")[0].strip()
    ba_3 = extract_to_end(s300, "INSERT INTO BookAuthors").split("-- ==================== BOOK CATEGORIES")[0].strip()
    if ba_f.endswith(";"):
        ba_f = ba_f[:-1].strip()
    if ba_3.endswith(";"):
        ba_3 = ba_3[:-1].strip()
    out.append(ba_f + ",")
    out.append(ba_3)
    out.append(";")
    out.append("")

    # 9) BookCategories: fake + 300
    out.append("-- ==================== BOOK CATEGORIES ====================")
    bc_f = extract_to_end(fake, "INSERT INTO BookCategories (BookId, CategoryId)").split("-- ==================== ORDERS")[0].strip()
    bc_3 = extract_to_end(s300, "INSERT INTO BookCategories (BookId, CategoryId)").split("\n\nPRINT")[0].strip()
    if bc_f.endswith(";"):
        bc_f = bc_f[:-1].strip()
    if bc_3.endswith(";"):
        bc_3 = bc_3[:-1].strip()
    out.append("INSERT INTO BookCategories (BookId, CategoryId) VALUES")
    out.append(bc_f + ",")
    out.append(bc_3)
    out.append(";")
    out.append("")

    # 10) Orders, OrderItems, Payments, Carts, CartItems, SellerProfiles from fake
    rest = extract_to_end(fake, "-- ==================== ORDERS (12)")
    out.append(rest.strip())
    out.append("")
    out.append("PRINT 'MockData.sql (merged seed) completed.';")

    with open(OUT, "w", encoding="utf-8") as f:
        f.write("\n".join(out))
    print("Written", OUT)

if __name__ == "__main__":
    main()
