#!/usr/bin/env python3
"""
Validate Assets/Resources/questions.json (syntax + basic sanity).
Does NOT verify math correctness — structure, index bounds, duplicates.

Usage (from repo root):
  python3 tools/validate_questions.py

From tools/ folder:
  python3 validate_questions.py
"""
from __future__ import annotations

import json
import re
import sys
from collections import defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
JSON_PATH = ROOT / "Assets" / "Resources" / "questions.json"


def norm(s: str) -> str:
    s = (s or "").strip().lower()
    return re.sub(r"\s+", " ", s)


def main() -> int:
    if not JSON_PATH.is_file():
        print(f"Missing: {JSON_PATH}", file=sys.stderr)
        return 1

    raw = JSON_PATH.read_text(encoding="utf-8")
    try:
        data = json.loads(raw)
    except json.JSONDecodeError as e:
        print(f"Invalid JSON: {e}", file=sys.stderr)
        return 1

    db = data.get("database", {})
    errors: list[str] = []
    warnings: list[str] = []
    cross: dict[tuple[str, str], list[str]] = defaultdict(list)

    for diff in ("easy", "medium", "hard"):
        items = db.get(diff) or []
        seen: dict[tuple[str, str], list[int]] = defaultdict(list)

        for i, q in enumerate(items):
            topic = (q.get("topic") or "").strip()
            text = q.get("question") or ""
            choices = q.get("choices") or []
            ai = q.get("answer_index")

            if not topic:
                errors.append(f"{diff}[{i}]: missing topic")
            if not str(text).strip():
                errors.append(f"{diff}[{i}]: empty question")
            if not isinstance(choices, list) or len(choices) == 0:
                errors.append(f"{diff}[{i}]: missing or empty choices")
            if not isinstance(ai, int):
                errors.append(f"{diff}[{i}]: answer_index must be int")
            elif choices and (ai < 0 or ai >= len(choices)):
                errors.append(
                    f"{diff}[{i}]: answer_index={ai} out of range (0..{len(choices)-1})"
                )

            key = (topic.lower(), norm(text))
            seen[key].append(i)
            cross[key].append(diff)

            # Weak hint: explanation admits the item is broken (LLM self-doubt)
            expl = (q.get("explanation") or "").lower()
            if "does not match" in expl or "not an option" in expl or "needs to be rephrased" in expl:
                warnings.append(
                    f"{diff}[{i}]: explanation may flag a bad question — review manually"
                )

        for key, indices in seen.items():
            if len(indices) > 1:
                topic, qpreview = key[0], (key[1][:60] + "…") if len(key[1]) > 60 else key[1]
                warnings.append(
                    f"{diff}: duplicate x{len(indices)} topic={topic!r} q={qpreview!r} indices={indices}"
                )

    # Same topic + same question text appears in multiple difficulties
    for key, diffs in cross.items():
        uniq = sorted(set(diffs))
        if len(uniq) > 1:
            topic, qpreview = key[0], (key[1][:50] + "…") if len(key[1]) > 50 else key[1]
            warnings.append(
                f"CROSS-DIFFICULTY duplicate: {topic!r} / {qpreview!r} in {uniq}"
            )

    print(f"Checked: {JSON_PATH}")
    if errors:
        print("\nERRORS:")
        for e in errors:
            print(" ", e)
    if warnings:
        print("\nWARNINGS:")
        for w in warnings[:120]:
            print(" ", w)
        if len(warnings) > 120:
            print(f"  ... and {len(warnings) - 120} more")

    if not errors and not warnings:
        print("OK: no structural errors; no duplicate topic+question within difficulty.")
        print("     (Cross-difficulty duplicates are OK — Unity now avoids repeating them in one run.)")
        return 0
    if errors:
        return 1
    print("\nExit 0: warnings only — review flagged lines in questions.json.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
