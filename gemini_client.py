import os
from dotenv import load_dotenv
from google import genai


def get_gemini_client() -> tuple[genai.Client, str]:
    """
    Returns (client, model_name).
    Loads GEMINI_API_KEY and GEMINI_MODEL from .env or environment variables.
    """
    load_dotenv()

    api_key = os.getenv("GEMINI_API_KEY")
    model = os.getenv("GEMINI_MODEL", "gemini-2.5-flash")

    if not api_key:
        raise RuntimeError(
            "Missing GEMINI_API_KEY. Create a .env file (see .env.example) or set it in your environment."
        )

    client = genai.Client(api_key=api_key)
    return client, model