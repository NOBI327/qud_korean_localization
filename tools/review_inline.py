#!/usr/bin/env python3
"""Review inline translation scripts for quality issues"""
import re
import importlib.util
import sys
import os

def load_translations(script_path):
    """Load TRANSLATIONS dict from a script file"""
    spec = importlib.util.spec_from_file_location("module", script_path)
    mod = importlib.util.module_from_spec(spec)
    # Only extract TRANSLATIONS, don't execute main
    with open(script_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Use exec to get TRANSLATIONS
    namespace = {}
    # Find and exec just the TRANSLATIONS dict
    lines = content.split('\n')
    in_dict = False
    dict_lines = []
    brace_count = 0
    for line in lines:
        if 'TRANSLATIONS' in line and '=' in line and '{' in line:
            in_dict = True
        if in_dict:
            dict_lines.append(line)
            brace_count += line.count('{') - line.count('}')
            if brace_count <= 0 and len(dict_lines) > 1:
                break

    dict_code = '\n'.join(dict_lines)
    exec(dict_code, namespace)
    return namespace.get('TRANSLATIONS', {})

def review(translations, group_name):
    print(f"\n=== {group_name}: {len(translations)} translations ===")

    issues = []

    for orig, kr in translations.items():
        # 1. Template variable check
        orig_vars = set(re.findall(r'=[a-zA-Z][\w.]*=', orig))
        kr_vars = set(re.findall(r'=[a-zA-Z][\w.]*=', kr))
        if orig_vars != kr_vars:
            missing = orig_vars - kr_vars
            extra = kr_vars - orig_vars
            issues.append(('VAR', orig[:60], f'missing={missing} extra={extra}'))

        # 2. Color code check
        orig_cc = re.findall(r'\{\{[A-Za-z]+\|', orig)
        kr_cc = re.findall(r'\{\{[A-Za-z]+\|', kr)
        if len(orig_cc) != len(kr_cc):
            issues.append(('COLOR', orig[:60], f'orig={len(orig_cc)} kr={len(kr_cc)}'))

        # 3. Empty translation
        if not kr.strip():
            issues.append(('EMPTY', orig[:60], ''))

        # 4. Untranslated (korean same as original)
        if kr == orig and len(orig) > 10:
            issues.append(('UNTRANSLATED', orig[:60], ''))

        # 5. Tilde count mismatch (choice variants)
        if '~' in orig:
            if orig.count('~') != kr.count('~'):
                issues.append(('TILDE', orig[:60], f'orig={orig.count("~")} kr={kr.count("~")}'))

    if issues:
        print(f"  ISSUES FOUND: {len(issues)}")
        for typ, text, detail in issues:
            print(f"  [{typ}] {text} | {detail}")
    else:
        print("  No technical issues found.")

    # Tone check: sample some translations
    print(f"\n  Sample translations (first 10):")
    for i, (orig, kr) in enumerate(translations.items()):
        if i >= 10:
            break
        print(f"    EN: {orig[:70]}")
        print(f"    KR: {kr[:70]}")
        print()

    return len(issues)

if __name__ == "__main__":
    total_issues = 0
    for gid in [1, 2]:
        path = f"tools/apply_inline_{gid}.py"
        if os.path.exists(path):
            try:
                tr = load_translations(path)
                total_issues += review(tr, f"Group {gid}")
            except Exception as e:
                print(f"Error loading {path}: {e}")

    print(f"\n=== Total issues: {total_issues} ===")
