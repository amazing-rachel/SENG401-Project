# Testing Artifact – 3D Snakes & Ladders: Quality Education Edition

## Intro

This testing artifact demonstrates that the **core functionality of the game works as intended**. It contains **systematic test cases**, **test data**, **expected and actual results**, and **validation notes**. Tests cover all critical features, including **account management, gameplay mechanics, question answering, database interactions, and accessibility options**.  

## Test Cases

| Test Case ID | Feature | Input / Action | Expected Result | Actual Result | Status | Validation Notes |
|-------------|---------|----------------|----------------|---------------|--------|-----------------|
| TC-001 | Account Creation | username=`Aisha123`, password=`Password1!` -> Submit | Account is created and saved in database | Account created; SQL table `users` contains username `Aisha123` with correct default scores and progress | Pass | Confirms account creation logic works and database is properly connected. |
| TC-002 | Login | username=`Aisha123`, password=`Password1!` -> Login | User accesses profile page | User successfully navigates to profile page with correct scores and progress displayed | Pass | Verifies authentication system and correct retrieval from database. |
| TC-003 | Profile Viewing | Click “View Profile” | Profile shows username, saved scores, and game progress | All profile information displayed accurately, scores match database entries | Pass | Confirms profile retrieval works correctly. |
| TC-004 | Profile Editing | Change username=`AishaGamer` -> Save | Database and UI update | Username updated in UI and SQL `users` table | Pass | Ensures profile editing updates both frontend and backend consistently. |
| TC-005 | Dice Roll | Click “Roll Die” | Dice shows 1–6, player token moves accordingly | Die rolled 4; token moved 4 spaces from 6 -> 10; score panel updated correctly | Pass | Validates dice randomness logic and correct movement mechanics. |
| TC-006 | Ladder Climb | Answer applied-knowledge question correctly at ladder | Token climbs ladder correctly | Token moved from space 6 -> 14 after correct answer; feedback displayed: “Ladder climbed!” | Pass | Confirms ladder logic triggers correctly upon correct answer. |
| TC-007 | Snake Slide | Player lands on snake | Token slides down snake | Token moved from space 18 -> 12; feedback displayed: “Oops! You slid down the snake” | Pass | Ensures snake logic functions as intended. |
| TC-008 | Basic Question Answering | Answer multiple-choice question correctly | Score increases; feedback “Correct!” | Score increased by 10; feedback displayed: “Correct! +10 points” | Pass | Validates question-answer logic and immediate feedback system. |
| TC-009 | Applied Knowledge Question | Answer applied question incorrectly | Cannot climb ladder; feedback “Incorrect, try next turn” | Token stayed at base of ladder; feedback displayed: “Incorrect, try next turn” | Pass | Confirms applied knowledge restriction works correctly. |
| TC-010 | Critical Thinking Question | Answer critical question correctly | Avoid snake; feedback displayed | Token avoided snake; feedback: “Critical thinking saved you!” | Pass | Ensures critical thinking mechanism works for player advantage. |
| TC-011 | Educational Topic Selection | Select “Math & Logic” | Questions related to chosen topic load | 3 multiple-choice and 2 fill-in-the-blank questions loaded for Math & Logic | Pass | Validates topic selection and question retrieval from backend. |
| TC-012 | Multiplayer Move | Player 1 rolls die | Only Player 1 moves; turn switches | Player 1 token moved 3 spaces; Player 2 token remained; next turn active | Pass | Confirms multiplayer turn logic works correctly. |
| TC-013 | Save Game Progress | Quit game | Progress saved in database | SQL `game_progress` table updated with token position 10, score 20 | Pass | Ensures game progress persistence works correctly. |
| TC-014 | Accessibility | Increase text size, large buttons | UI adjusts accordingly | UI text increased, buttons enlarged; all elements readable | Pass | Confirms accessibility settings are applied correctly. |
| TC-015 | Database Interaction | Fetch past scores | Correct scores retrieved | SQL query returned previous scores; UI displayed values match database | Pass | Validates proper retrieval of stored data. |


## Test Data

- **Usernames / Passwords:** `Aisha123` / `Password1!`, `TestUser` / `Test123!`  
- **Dice Rolls:** 1, 2, 3, 4, 5, 6  
- **Sample Questions:**  
  - **Basic:** “What is 2+2?” -> Answer: 4  
  - **Applied:** “Solve 3x+5=11, find x” -> Answer: 2  
  - **Critical:** “Explain why photosynthesis is vital for humans” -> LLM evaluates answer as correct or incorrect  
- **Gameboard positions:** start, ladder base, ladder top, snake head, snake tail  

## 4. Validation Notes

- **Gameplay Mechanics:** Dice rolls, ladder climbs, snake slides, and token movements confirmed.  
- **Question System:** All question types correctly validate answers and update scores.  
- **Database:** Account creation, login, profile editing, game progress, and score retrieval tested successfully.  
- **Accessibility:** UI responds correctly to large text and buttons adjustments.  
- **Multiplayer Logic:** Player turns and token movements work as expected.  

